import json
from cwa_ClosedLoopAPI import *

def lambda_handler(event, context):
    try:
        input_msg = json.loads(event['Records'][0]['Sns']['Message'])
        msg_attributes = event['Records'][0]['Sns']['MessageAttributes']
        print('Input Message Received : - ' + json.dumps(input_msg))
        print('Calling CWA Closed Loop API for payload_id : ' + input_msg['payload_id'] + ', check_count : ' + str(len(input_msg['remediated_checks'])))
        updateRemediationPayloadChecks(input_msg)
    
        return {
            'statusCode': 200,
            'body': json.dumps('Successfully called CWA Closed Loop API')
        }
    except Exception as ex :
        print('Error Occured while calling CWA CLosed Loop API ' + str(ex))
        return {
            'statusCode': 500,
            'body': json.dumps('Error Occured while calling CWA CLosed Loop API ' + str(ex))
        }
        
