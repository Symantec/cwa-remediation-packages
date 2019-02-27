import json
import boto3
import sys
from raise_sns_notification import *

def lambda_handler(event, context):
    remediation_check_map = dict()
    cloud_provider_config = {}
    try:
        print('symc_cwa_remediation_worker_s3_called')
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

        s3VersionEnabled = False

        s3VersionenabledCheck='282ac909-af9c-4c8e-81d7-e1c23533378b'

        for check in event['checks']:
            if(check['id'].lower()== s3VersionenabledCheck):
                if(generateAndSendRemediationOutput):
                    remediation_check_obj = {}
                    remediation_check_map[s3VersionenabledCheck]=remediation_check_obj
                    remediation_check_obj['id']=check['id']
                    remediation_check_obj['status']=0
                    remediation_check_obj['version']=check['version']
                s3VersionEnabled = True


        try:
            if(s3VersionEnabled):
                print('Get S3 Bucket')
                S3Bucket = str(event['resource']['id'])
                print('Get S3 Resource of the bucket')
                s3 = boto3.resource('s3')
                print('Enable Bucket versioning for - ' + S3Bucket)
                bucket_versioning = s3.BucketVersioning(S3Bucket)
                response = bucket_versioning.enable();
                print("response from bucket versionsing  - " + str(response))


            if(generateAndSendRemediationOutput):
                for k in remediation_check_map:
                    if(k.lower() == s3VersionenabledCheck):
                        remediation_check_obj = remediation_check_map[s3VersionenabledCheck]
                        remediation_check_obj['status']=0
                        remediation_check_obj['message']='Remediated Succesfully from AWS Lambda'
                        remediation_output_message["remediated_checks"].append(remediation_check_obj)

            print('Remediation successful.')

        except:
            type, value, traceback = sys.exc_info()
            print(value)
            print('Error occurred while calling S3 API ' + str(sys.exc_info()[0]))
            errorMessage = str(sys.exc_info()[0])

            for k in remediation_check_map:
                if(k.lower() == s3VersionenabledCheck):
                    remediation_check_obj = remediation_check_map[s3VersionenabledCheck]
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
