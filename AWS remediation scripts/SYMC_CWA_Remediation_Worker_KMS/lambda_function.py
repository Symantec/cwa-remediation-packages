import json
import boto3
import sys
from raise_sns_notification import *

def lambda_handler(event, context):
    remediation_check_map = dict()
    cloud_provider_config = {}
    try:
        print('symc_cwa_remediation_worker_kms_called')
        print(event)
        print('Remediating '+event['resource']['id']+' in region '+event['resource']['region'])

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

        enable_KeyRotaion = False

        enablekeyRotaionCheck='19772abf-4fef-4b2e-a2c6-b2b8205fb0db'

        for check in event['checks']:
            if(check['id'].lower()== enablekeyRotaionCheck):
                if(generateAndSendRemediationOutput):
                    remediation_check_obj = {}
                    remediation_check_map[enablekeyRotaionCheck]=remediation_check_obj
                    remediation_check_obj['id']=check['id']
                    remediation_check_obj['status']=0
                    remediation_check_obj['version']=check['version']
                enable_KeyRotaion = True


        try:
            print('Getting KMS key ID')
            kmsKeyID = str(event['resource']['id'])
            print('Creating client of KMS' )
            Kms = boto3.client('kms')
            print('Enable Key Rotation for ' + str(kmsKeyID))
            if(enable_KeyRotaion):
                print('Enabling Key Rotation')
                response = Kms.enable_key_rotation( KeyId=kmsKeyID)
                print(response)
                print('Key rotation enabled')

            if(generateAndSendRemediationOutput):
                for k in remediation_check_map:
                    if(k.lower() == enablekeyRotaionCheck):
                        remediation_check_obj = remediation_check_map[enablekeyRotaionCheck]
                        remediation_check_obj['status']=0
                        remediation_check_obj['message']='Remediated Succesfully from AWS Lambda'
                        remediation_output_message["remediated_checks"].append(remediation_check_obj)

            print('Remediation successful.')

        except:
            type, value, traceback = sys.exc_info()
            print(value)
            print('Error occurred while calling KMSupdate API ' + str(sys.exc_info()[0]))
            errorMessage = str(sys.exc_info()[0])

            for k in remediation_check_map:
                if(k.lower() == enablekeyRotaionCheck):
                    remediation_check_obj = remediation_check_map[enablekeyRotaionCheck]
                    remediation_check_obj['status']=1
                    remediation_check_obj['message']=errorMessage
                    remediation_output_message["remediated_checks"].append(remediation_check_obj)

        if(generateAndSendRemediationOutput):
            remediation_output_message_str = json.dumps(remediation_output_message)
            print('Remediation Output Message Generated : ' + remediation_output_message_str)
            remediation_output_message_str_json = prepareSNSJsonMessage(remediation_output_message_str)
            raise_sns_notification(cloud_provider_config['output_sns_topic_arn'],cloud_provider_config['output_sns_topic_region'],remediation_output_message_str_json,'json')
            print('Remediation Output Message Published Succesfully to SNS.')

        return {
            'statusCode': 200,
            'body': json.dumps('Remediation Performed Succesfully')
        }
    except:
        print ("Unexpected error:", str(sys.exc_info()[0]))
        raise
