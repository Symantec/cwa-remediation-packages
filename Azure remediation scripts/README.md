
# Cloud Workload Assurance Azure Remediation


## Getting Started:
* Use an active Azure account
* For deploying the code, you can use one of the following methods:
	
	* Downlaod the CWAFunctionApp.zip and manually deploy the function app using the zip-push method. 
	  Refer to link [Azure zip-push deployment](https://docs.microsoft.com/en-us/azure/azure-functions/deployment-zip-push)	 

	* Use the option: 
	
	  [![Deploy to Azure](https://azuredeploy.net/deploybutton.svg)](https://azuredeploy.net/)
	
	  and follow the steps given below.

## Steps to Follow:
* Fork the "Azure remediation scripts" repository to your local GitHub repository.

* Click "Deploy to Azure" Button
* On the Deploy to Azure page, enter the following information and click Next:
	* Directory
	* Resource Group - you can either use an existing one or enter a new one
	* Site Name
	* Subscription
	* Resource Group Name
	* Location - You can have one function app in one location.
* Click Deploy.
* Configure CLIENT_ID, CLIENT_SECRET, DIRECTORY_ID varibles under configuration.
* Add event grid subscription as mentioned in remediation steps.
* Compile the code (optional)

For further details about Azure Remediation Steps, refer to the [documentation link](https://help.symantec.com/cs/SCWP/SCWA/v131901146_v127279924/title?locale=EN_US&sku=CWA)
	
-----------------------------------------------------------------------------------------------------------------------------------
**Copyright © 2019 Symantec Corporation. All rights reserved.**

Symantec, the Symantec Logo, the Checkmark Logo and  are trademarks or registered trademarks of Symantec Corporation or its affiliates in the U.S. and other countries. Other names may be trademarks of their respective owners.

This Symantec product may contain third party software for which Symantec is required to provide attribution to the third party (“Third Party Programs”). Some of the Third Party Programs are available under open source or free software licenses. The License Agreement accompanying the Software does not alter any rights or obligations you may have under those open source or free software licenses. Please see the Third Party Legal Notice Appendix to this Documentation or TPIP ReadMe File accompanying this Symantec product for more information on the Third Party Programs.

The product described in this document is distributed under licenses restricting its use, copying, distribution, and decompilation/reverse engineering. No part of this document may be reproduced in any form by any means without prior written authorization of Symantec Corporation and its licensors, if any.

THE DOCUMENTATION IS PROVIDED "AS IS" AND ALL EXPRESS OR IMPLIED CONDITIONS, REPRESENTATIONS AND WARRANTIES, INCLUDING ANY IMPLIED WARRANTY OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE OR NON-INFRINGEMENT, ARE DISCLAIMED, EXCEPT TO THE EXTENT THAT SUCH DISCLAIMERS ARE HELD TO BE LEGALLY INVALID. SYMANTEC CORPORATION SHALL NOT BE LIABLE FOR INCIDENTAL OR CONSEQUENTIAL DAMAGES IN CONNECTION WITH THE FURNISHING, PERFORMANCE, OR USE OF THIS DOCUMENTATION. THE INFORMATION CONTAINED IN THIS DOCUMENTATION IS SUBJECT TO CHANGE WITHOUT NOTICE.

The Licensed Software and Documentation are deemed to be commercial computer software as defined in FAR 12.212 and subject to restricted rights as defined in FAR Section 52.227-19 "Commercial Computer Software - Restricted Rights" and DFARS 227.7202, et seq. "Commercial Computer Software and Commercial Computer Software Documentation," as applicable, and any successor regulations, whether delivered by Symantec as on premises or hosted services. Any use, modification, reproduction release, performance, display or disclosure of the Licensed Software and Documentation by the U.S. Government shall be solely in accordance with the terms of this Agreement.

Symantec Corporation
350 Ellis Street
Mountain View, CA 94043
https://www.symantec.com

