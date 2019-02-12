import json
import boto3
import sys
import os
import copy
from default_remediate import *

def lambda_handler(event, context):
    try:
        input_msg = json.loads(event['Records'][0]['Sns']['Message'])
        msg_attributes = event['Records'][0]['Sns']['MessageAttributes']
        process(input_msg,msg_attributes,context)
            
        return {
            'statusCode': 200,
            'body': json.dumps('Successfully called worker lambdas')
        }
    except:
        print ("Unexpected error:", sys.exc_info()[0])    
        raise

def process(message, attributes, context):
    print('Remediation processing function called')
    try:
        print(message)
        #print(msg_attributes)
        orchestratorLambdaAccountId = context.invoked_function_arn.split(":")[4]
        print("orchestratorLambdaAccountId :- " + orchestratorLambdaAccountId)
        workerLambdaRegion = ""
        workerLambdaRegion = os.getenv("WorkerLambdaRegion","")
        if workerLambdaRegion == "" :
            print("workerLambdaRegion is not specified")
        else:
            print("workerLambdaRegion :- " + workerLambdaRegion)
        
        
        filtered_payload = split_payload_for_workers(message,orchestratorLambdaAccountId,workerLambdaRegion)
        client = boto3.client('lambda')
        
        print("Invoking worker lambdas")
        for key in filtered_payload:
            if key!="default":
                client.invoke(
                    FunctionName=key,
                    InvocationType='Event',
                    LogType='None',
                    Payload=json.dumps(filtered_payload.get(key))
                    )
                print(key)
            else:
                do_remediation(filtered_payload.get(key))
    except:
        print ("Unexpected error:", sys.exc_info()[0])    
        raise

def split_payload_for_workers(message,orchestratorLambdaAccountId,workerLambdaRegion):
    print('Splitting payload to distribute amongst workers')
    default_remediation_function = "default"
    check_function_mapping = {}
    distributed_payload = {}
    exists = os.path.isfile('check_config.json')
    fileconfig = None
    if exists:
        with open('check_config.json') as json_config:
            fileconfig = json.load(json_config)
            print(fileconfig)
    else:
        print('No check config file found')
        
    workerLambdaAccountIdToken = "%%workerLambdaAccountId%%" 
    workerLambdaRegionToken = "%%workerLambdaRegion%%"
    
        
    for chk in message['checks']:
        function_name = fileconfig.get(chk['id'].lower(),default_remediation_function)
        print("function_name from fileConfig :- " + function_name)
        if function_name.find(workerLambdaAccountIdToken) and orchestratorLambdaAccountId != "":
            function_name = function_name.replace(workerLambdaAccountIdToken,orchestratorLambdaAccountId)
        if function_name.find(workerLambdaRegionToken) and  workerLambdaRegion != "" :
            function_name = function_name.replace(workerLambdaRegionToken,workerLambdaRegion)
        print("function_name to be called finally : - " + function_name)
        if function_name in check_function_mapping:
            check_function_mapping[function_name].append(chk['id'])
        else:
            chk_list = []
            chk_list.append(chk['id'])
            check_function_mapping[function_name] = chk_list
            
        if chk['id'] in fileconfig:
            print('Remediation worker "'+ function_name +'" configured for check "'+ chk['name']+'"')
        else:
            print('No Remediation worker configured for check "'+ chk['name']+'"')
    
    for key,value in check_function_mapping.items():
        filtered_payload = message
        #filtered_payload = copy.deepcopy(message)
        #TODO: Remove checks from payload which are not part of value list (mappings)
        distributed_payload[key] = filtered_payload
        
    return distributed_payload
        
