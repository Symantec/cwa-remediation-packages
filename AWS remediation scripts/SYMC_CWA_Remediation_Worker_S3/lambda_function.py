#!/usr/bin/python
# -*- coding: utf-8 -*-
import json
import boto3
import copy
import sys
import time
from raise_sns_notification import *

def enable_encryption_on_bucket(s3bucket):
    client = boto3.client('s3')
    print ('Enable Bucket encryption for - ' + s3bucket)
    response = client.put_bucket_encryption(Bucket=s3bucket, ServerSideEncryptionConfiguration={'Rules':[
        {'ApplyServerSideEncryptionByDefault': {'SSEAlgorithm': 'AES256'}}]})
    print ('response from bucket encryption  - ' + str(response))
    # check if bucket is encrypted
    bucket_encrypt_status = get_bucket_encryption(client, s3bucket)
    if response['ResponseMetadata']['HTTPStatusCode'] == 200 and bucket_encrypt_status:
        return True
    else:
        return False
        
def enable_version_on_bucket(s3bucket):
    s3 = boto3.resource('s3')
    print('Enable Bucket versioning for - ' + s3bucket)
    bucket_versioning = s3.BucketVersioning(s3bucket)
    response = bucket_versioning.enable();
    print ('response from bucket versioning  - ' + str(response))
    if response['ResponseMetadata']['HTTPStatusCode'] == 200:
        return True
    else:
        return False
    
def return_bucket(event):
    try:
        return str(event['resource']['id'])
    except KeyError:
        return None

def get_bucket_encryption(client, s3bucket):
    result = None
    for i in range(0,5):
        time.sleep(i)
        try:
            result = client.get_bucket_encryption(Bucket=s3bucket)
        except:
            pass
        if result:
            print("Bucket encryption policy after encryption: {}"
                  .format(result['ServerSideEncryptionConfiguration']['Rules']))
            break
    return result

def lambda_handler(event, context):
    remediation_status = None
    # map of functions and check ids
    check_id_to_func = {'c2ec4814-76c4-4413-840f-b2b0766d0077': enable_encryption_on_bucket,
                        '282ac909-af9c-4c8e-81d7-e1c23533378b': enable_version_on_bucket}
    # used to generate ouput
    remediation_output = {"payload_id": None, 
                          "remediated_checks": []}
    remediation_check = {"id": None, "status": 0, "version": "1.0.0", 
                                                  "message": "Remediated Succesfully from AWS Lambda"}
    
    # start a counter
    counter = 0
    
    bucket_to_act_on = return_bucket(event)
    # create a list of checks
    check_id = [x for x in event['checks']]
    print ('symc_cwa_remediation_worker_s3_called')
    print ('Remediating ' + event['resource']['id'] + ' in region ' + event['resource']['region'])
    try:
        # perform the remediation, store all statuses
        remediation_status = []
        for check in check_id:
            # run functions and populate with the return
            remediation_status.append(check_id_to_func[check['id']](bucket_to_act_on))
    except Exception as e:
        print ('Something went wrong: {}'.format(e))
     
    # populate data into output
    remediation_output['payload_id'] = event['payload_id']
    # populate the remediation output with multiple checks, only if successful
    for x in remediation_status:
        if x is (not None)  and (not False):
            # set payload to insert for each check
            _remediation_check = remediation_check
            _remediation_check['id'] = check_id[counter]['id']
            # using copy, otherwise reference is set for all checks to last one
            remediation_output['remediated_checks'].append(copy.copy(_remediation_check))
        counter += 1
    print("Remediated Output Message Generated: {}".format(remediation_output))
    
    #  only perform if output sns topic is provided
    if event.get('cloud_provider_config'):
        sns_message = prepareSNSJsonMessage(json.dumps(remediation_output))
        raise_sns_notification(event['cloud_provider_config']['output_sns_topic_arn'],
                               event['cloud_provider_config']['output_sns_topic_region'],
                               sns_message, 'json')
        print ('Remediation Output Message Published Succesfully to SNS.')
    return {'statusCode': 200, 'body': 'Remediation Performed Succesfully'}