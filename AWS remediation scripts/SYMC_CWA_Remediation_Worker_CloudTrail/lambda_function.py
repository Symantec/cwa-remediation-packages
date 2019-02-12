import json
import boto3
import sys
from raise_sns_notification import *

def lambda_handler(event, context):
    remediation_check_map = dict()
    cloud_provider_config = {}
    try:
        print('symc_cwa_remediation_worker_cloudtrail_called')
        print(event)
        print('Remediating '+event['resource']['name']+' in region '+event['resource']['region'])

        generateAndSendRemediationOutput = False
        for eventKeys in event:
            if(eventKeys == "cloud_provider_config"):
                generateAndSendRemediationOutput = True
                cloud_provider_config = event['cloud_provider_config']
                print (eventKeys)
                break

        if(generateAndSendRemediationOutput):
            print('Remediation Cloud Provider Configuration : ' + json.dumps(cloud_provider_config))
        else:
            print('Remediation Cloud Provider Configuration Not Provided')


        remediation_output_message = {}
        if(generateAndSendRemediationOutput):
            remediation_output_message["payload_id"] = event['payload_id']
            remediation_output_message["remediated_checks"] = []

        cloudTrail_enableInAllregion = False

        enableAllregionCheck='7952ef64-5262-4829-bbb9-17e36b68cd22'

        for check in event['checks']:
            if(check['id'].lower()== enableAllregionCheck):
                if(generateAndSendRemediationOutput):
                    remediation_check_obj = {}
                    remediation_check_map[enableAllregionCheck]=remediation_check_obj
                    remediation_check_obj['id']=check['id']
                    remediation_check_obj['status']=0
                    remediation_check_obj['version']=check['version']
                cloudTrail_enableInAllregion = True


        try:
            print('Getting CloudTrail Name')
            cloudTrailName = str(event['resource']['name'])
            print('Creating client of Cloud Trail' )
            cloudtrail_Client = boto3.client('cloudtrail')
            print('Enabling Cloudtrail for - ' + str(cloudTrailName))
            if(cloudTrail_enableInAllregion):
                print('Enabling CloudTrail for all regions')
                response = cloudtrail_Client.update_trail(Name=cloudTrailName, IsMultiRegionTrail=True, IncludeGlobalServiceEvents=True)
                print(response)
                print('Cloud trail enabled')

            if(generateAndSendRemediationOutput):
                for k in remediation_check_map:
                    if(k.lower() == enableAllregionCheck):
                        remediation_check_obj = remediation_check_map[enableAllregionCheck]
                        remediation_check_obj['status']=0
                        remediation_check_obj['message']='Remediated Succesfully from AWS Lambda'
                        remediation_output_message["remediated_checks"].append(remediation_check_obj)

            print('Remediation successful.')

        except:
            type, value, traceback = sys.exc_info()
            print(value)
            print('Error occurred while calling Cloud trail update API ' + str(sys.exc_info()[0]))
            errorMessage = str(sys.exc_info()[0])

            for k in remediation_check_map:
                if(k.lower() == enableAllregionCheck):
                    remediation_check_obj = remediation_check_map[enableAllregionCheck]
                    remediation_check_obj['status']=1
                    remediation_check_obj['message']=errorMessage
                    remediation_output_message["remediated_checks"].append(remediation_check_obj)

        if(generateAndSendRemediationOutput):
            remediation_output_message_str = json.dumps(remediation_output_message)
            print('Remediation Output Message Generated : ' + remediation_output_message_str)
            remediation_output_message_str_json = prepareSNSJsonMessage(remediation_output_message_str)
            raise_sns_notification(cloud_provider_config['output_sns_topic_arn'],cloud_provider_config['output_sns_region'],remediation_output_message_str_json,'json')
            print('Remediation Output Message Published Succesfully to SNS.')

        return {
            'statusCode': 200,
            'body': json.dumps('Remediation Performed Succesfully')
        }
    except:
        print ("Unexpected error:", str(sys.exc_info()[0]))
        raise
