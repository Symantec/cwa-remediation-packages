import json
from botocore.vendored import requests
import configparser
import os
from pathlib import Path

def updateRemediationPayloadChecks(payload):
    config_file = 'config.ini'
    AUTHURL = 'AuthUrl'
    CLIENT_ID = 'ClientId'
    CLIENT_SECRET = 'ClientSecretKey'
    CONFIG_CREDS_SECTION = 'Credentials'
    CONFIG_URL_SECTION = 'RequestURL'
    UPDATE_REMEDIATION_PAYLOAD_URL = 'UpdateRemediatinPayloadUrl'

    exists = os.path.isfile(config_file)
    if exists:
        print('Config file : ' + config_file +  ' found')
    else:
        print('Unable to load configuration, File : ' + config_file + ' is missing.')
        raise Exception('Unable to load configuration, config.ini is missing.')
    
    config = configparser.ConfigParser()
    config.read(config_file)
    
    auth_request = {}
    auth_headers = {}
    auth_headers_update_status = {}
    
    authurl = config.get(CONFIG_URL_SECTION, AUTHURL)
    #client_id = config.get(CONFIG_CREDS_SECTION, CLIENT_ID)
    client_id = os.getenv("ClientID","")
    #client_secret = config.get(CONFIG_CREDS_SECTION, CLIENT_SECRET)
    client_secret = os.getenv("ClientSecretKey","")
    
    if client_id == "" or client_secret == "" :
        raise Exception("ClientID and/or ClientSecretKey in enviornment variables are not specified.")
    
    updateRemediationPayloadURL = config.get(CONFIG_URL_SECTION,UPDATE_REMEDIATION_PAYLOAD_URL)
    
    if client_id == "" or client_secret == "" or authurl == "" or updateRemediationPayloadURL == "":
        raise Exception("One or more values are empty in " + config_file)

    auth_request['client_id'] = client_id
    auth_request['client_secret'] = client_secret
    auth_headers['Content-type'] = 'application/json'
    auth_request_json = json.dumps(auth_request)
    payload_json = json.dumps(payload)
    auth_response = requests.post(authurl, data=auth_request_json, headers=auth_headers)
    if auth_response.status_code == 200:
        print("auth token generated successfully, " + "http status code is " + str(auth_response.status_code))
        print("auth response json : " + json.dumps(auth_response.json()))
        access_token = auth_response.json()['access_token']
        x_epmp_customer_id = auth_response.json()['x-epmp-customer-id']
        x_epmp_domain_id = auth_response.json()['x-epmp-domain-id']
        print("access_token :: " + access_token)
        print("customer_id :: " + x_epmp_customer_id)
        print("domain_id :: " + x_epmp_domain_id)
        auth_headers_update_status['Authorization'] = access_token
        auth_headers_update_status['x-epmp-customer-id'] = x_epmp_customer_id
        auth_headers_update_status['x-epmp-domain-id'] = x_epmp_domain_id
        auth_response_update_status = requests.post(updateRemediationPayloadURL,data=payload_json,headers=auth_headers_update_status)
        if auth_response_update_status.status_code == 200:
            print("Remediation Payload updated successfully")
            print("Returned Message : " + auth_response_update_status.text)
        else:
            raise Exception("Error while updating remediation Payload, HttpStatusCode : " + str(auth_response_update_status.status_code) + ", Details:- " + auth_response_update_status.text)
    else:
        raise Exception("Failed to generate auth token, " + "http status code is " + str(auth_response.status_code) + " , " + auth_response.text)