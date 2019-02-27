import boto3
import json
import sys

def prepareSNSJsonMessage(message):
    try:
        outputMessage = dict()
        outputMessage['default']=message
        outputMessage['lambda']=message
        return json.dumps(outputMessage)
    except Exception as e:
        print("Error Occurred while preparing SNS Json Message : " + str(e))
        raise

def raise_sns_notification(output_sns_topic_arn,output_sns_region,payload,payloadType):
    try:
        print("Raising SNS Notification for, output_sns_topic_arn : " + output_sns_topic_arn + " - output_sns_topic_region : " + output_sns_region)
        clientSNS = boto3.client('sns',output_sns_region)
        clientSNS.publish(
            TopicArn = output_sns_topic_arn , 
            Message = payload,
            MessageStructure = 'json' if  payloadType.lower() == 'json' else '')
        print("Raised SNS Notification Succesfully")
    except:
        print('Error Occurred while publishing message to SNS Topic ' + output_sns_region + ', Details:- ' + str(sys.exc_info()[0]))
        raise