==================================================================================================
										   README.txt

						POWERENV - IBM POWER5 SYSTEMS MANAGEMENT ENVIRONMENT

									  AUTHOR: NeuroCyberTron
==================================================================================================

=======================================SERIAL COMMUNICATIONS=======================================

			   >_IMPORTANT STUFF ABOUT INITIALIZING A SERIAL CONNECTION IN POWERENV:

	-- THE SERIAL PORT MUST BE SET TO 19200 BAUD RATE, 8 DATA BITS, NO PARITY, 1 STOP BIT, AND NO
											FLOW CONTROL.

	--     THE SERIAL PORT MUST BE OPENED IN RAW MODE (NO LINE ENDINGS OR SPECIAL CHARACTERS).

	--           THE SERIAL PORT MUST BE CLOSED AFTER USE TO AVOID LOCKING THE PORT.

	--  THE SERIAL PORT MUSN'T BE USED BY OTHER PROGRAMS WHILE POWERENV IS RUNNING (THE PORT MUST
											  BE FREE).

							========================================

			   >_IMPORTANT STUFF ABOUT SENDING COMMANDS VIA SERIAL PORT IN POWERENV:

	 -- WHEN SENDING JUST INSTRUCTIONS (CHOOSING AN OPTION IN THE MENU), THE DEVICE DOES THE NEW
	 LINE OPERATION AUTOMATICALLY (YOU DON'T HAVE TO SEND THE '\n', JUST SEND THE COMMAND OPTION).

	 -- WHEN SENDING TEXT INPUTS (WHEN YOU ARE CHANGING THE VALUE OF A PROPERTY OR REAFIRMING YOUR
	   CHOICE), YOU HAVE TO SEND THE '\n' AT THE END OF THE INPUT, OTHERWISE IT WON'T FINISH THE
											  COMMAND.)
===================================================================================================