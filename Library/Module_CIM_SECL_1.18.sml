[Messages]
<S1F0 N Abort Transaction
>

<S1F1 P Are You There Request
>

<S1F2 S Online Data
  <L 2 
    <A 20 MDLN>
    <A 6 SOFTREV>
  >
>

<S1F3 P Selected Equipment State Request
  <L 2 
    <A 40 EQPID>
    <L n The Numbers of SVID
      <A 20 SVID>
    >
  >
>

<S1F4 S Selected Equipment State Data
  <L 2 
    <A 40 EQPID>
    <L n The Numbers of SVID L
      <L 2 SVID set
        <A 20 SVID>
        <A 200 SV>
      >
    >
  >
>

<S1F5 P Formatted State Request
  <L 2 
    <A 1 SFCD>
    <L n The Numbers of EQPID L
      <A 40 EQPID>
    >
  >
>

<S1F6 S Formatted State Data(SFCD 1 : Equipment State)
  <L 2 SFCD Set
    <A 1 SFCD>
    <L n EQP State Info L
      <L 2 EQP State Info
        <L 2 EQP Control State Set
          <A 40 EQPID>
          <A 1 CRST>
        >
        <L 9 EQP State Set
          <A 1 AVAILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
    >
  >
>

<S1F6 S Formatted State Data(SFCD 2 : Unit State)
  <L 2 SFCD Set
    <A 1 SFCD>
    <L n EQP State Info L
      <L 3 EQP State Info
        <L 2 EQP Control State Set
          <A 40 EQPID>
          <A 1 CRST>
        >
        <L 9 EQP State Set
          <A 1 AVAILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
        <L n Unit L
          <L 2 Unit Set
            <A 40 UNITID>
            <A 1 UNITST>
          >
        >
      >
    >
  >
>

<S1F6 S Formatted State Data(SFCD 3 : Material State)
  <L 2 SFCD Set
    <A 1 SFCD>
    <L n EQP State Info L
      <L 3 EQP State Info
        <L 2 EQP Control State Set
          <A 40 EQPID>
          <A 1 CRST>
        >
        <L 9 EQP State Set
          <A 1 AVAILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
        <L n Material L
          <L 5 Material Set
            <A 60 MATERIALID>
            <A 20 MATERIALTYPE>
            <A 1 MATERIALST>
            <A 1 MATERIALPORTID>
            <A 20 MATERIALUSAGE>
          >
        >
      >
    >
  >
>

<S1F6 S Formatted State Data(SFCD 4 : Equipment Port State)
  <L 2 SFCD Set
    <A 1 SFCD>
    <L n Port State Info L
      <L 2 Port State Info
        <L 2 EQP Control State Set
          <A 40 EQPID>
          <A 1 CRST>
        >
        <L n The Numbers of Port
          <L 7 Port Set
            <A 4 PORTNO>
            <A 1 PORTAVAILABLESTATE>
            <A 1 PORTACCESSMODE>
            <A 1 PORTTRANSFERSTATE>
            <A 1 PORTPROCESSINGSTATE>
            <A 20 REASONCODE>
            <A 40 DESCRIPTION>
          >
        >
      >
    >
  >
>

<S1F11 P Status Variable Name L Request
  <L 2 
    <A 40 EQPID>
    <L n The Numbers of SVID
      <A 20 SVID>
    >
  >
>

<S1F12 S Status Variable Name L Data
  <L 2 
    <A 40 EQPID>
    <L n The Numbers of SVID
      <L 2 SVID Set
        <A 20 SVID>
        <A 40 SVNAME>
      >
    >
  >
>

<S1F15 P Request OFF-LINE
  <L n The Numbers of EQP OFFLINE
    <L 2 EQP OFF Set
      <A 40 EQPID>
      <A 1 CRST>
    >
  >
>

<S1F16 S OFF-LINE Acknowledge
  <L n The Numbers of EQP OFFLINE
    <L 3 EQP OFF Set
      <A 40 EQPID>
      <A 1 CRST>
      <A 1 OFLACK>
    >
  >
>

<S1F17 P Request ON-LINE
  <L n The Numbers of EQP ONLINE
    <L 2 EQP ON set
      <A 40 EQPID>
      <A 1 CRST>
    >
  >
>

<S1F18 S ON-LINE Acknowledge
  <L n The Numbers of EQP ONLINE
    <L 3 EQP ON Set
      <A 40 EQPID>
      <A 1 CRST>
      <A 1 ONLACK>
    >
  >
>

<S2F0 N Abort Transaction
>

<S2F13 P New Equipment Constant Request
  <L 2 
    <A 40 EQPID>
    <L n The Number of EC L Count
      <A 8 ECID>
    >
  >
>

<S2F14 S New Equipment Constant Data
  <L n The Numbers of ECID
    <A 40 ECV1>
  >
>

<S2F15 P New Equipment Constant Send(ECS)
  <L 2 ECID Send Info
    <A 40 EQPID>
    <L n EC Set L
      <L 6 
        <A 8 ECID>
        <A 20 ECDEF>
        <A 20 ECSLL>
        <A 20 ECSUL>
        <A 20 ECWLL>
        <A 20 ECWUL>
      >
    >
  >
>

<S2F16 S New Equipment Constant Acknowledge
  <L 2 
    <A 40 EQPID>
    <L n The Numbers of ECID
      <L 8 
        <A 1 TEAC>
        <L 2 
          <A 8 ECID>
          <A 1 EAC>
        >
        <L 2 
          <A 40 ECNAME>
          <A 1 EAC>
        >
        <L 2 
          <A 40 ECDEF>
          <A 1 EAC>
        >
        <L 2 
          <A 40 ECSLL>
          <A 1 EAC>
        >
        <L 2 
          <A 40 ECSUL>
          <A 1 EAC>
        >
        <L 2 
          <A 40 ECWLL>
          <A 1 EAC>
        >
        <L 2 
          <A 40 ECWUL>
          <A 1 EAC>
        >
      >
    >
  >
>

<S2F17 P Data and Time Request
>

<S2F18 S Data and Time Data
  <A 14 TIME>
>

<S2F23 P Trace Initialize Send(TIS)
  <L 6 Trace Init Info Set
    <A 40 EQPID>
    <A 5 TRID>
    <A 6 DSPER>
    <A 5 TOTSMP>
    <A 3 REPGSZ>
    <L n The Numbers of SV Count
      <A 20 SVID>
    >
  >
>

<S2F24 S Trace Initialize Acknowledge
  <A 1 TIAACK>
>

<S2F29 P Equipment Constant NameL Request
  <L 2 
    <A 40 EQPID>
    <L n The Numbers of ECID
      <A 8 ECID>
    >
  >
>

<S2F30 S Equipment Constant Name Data
  <L 2 
    <A 40 EQPID>
    <L n The Numbers of ECID
      <L 7 ECID Set
        <A 8 ECID>
        <A 40 ECNAME>
        <A 20 ECDEF>
        <A 20 ECSLL>
        <A 20 ECSUL>
        <A 20 ECWLL>
        <A 20 ECWUL>
      >
    >
  >
>

<S2F31 P Data and Time Set Request(DTS)
  <A 14 TIME>
>

<S2F32 S Data and Time Set Acknowledge(DTA)
  <A 1 TIACK>
>

<S2F41 P HOST Command Send(HCS) (RCMD 1:Opcall Send)
  <L 2 RCMD Set
    <A 1 RCMD>
    <L 1 RCMD Type Set
      <L 4 Opcall Set
        <A 9 OPCALL>
        <A 40 EQPID>
        <A 20 OPCALLID>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F41 P HOST Command Send(HCS) (RCMD 2:Interlock Send)
  <L 2 RCMD Set
    <A 1 RCMD>
    <L 1 RCMD Type Set
      <L 4 Interlock Set
        <A 9 INTERLOCK>
        <A 40 EQPID>
        <A 20 INTERLOCKID>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F41 P HOST Command Send(HCS) (RCMD 3:Job(=PPID) Select)
  <L 2 RCMD Set
    <A 1 RCMD>
    <L n RCMD Type Set
      <L 4 Job Select Set
        <A 40 PPID>
        <A 40 EQPID>
        <A 4 PORTNO>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F41 P HOST Command Send(HCS) (RCMD 4,5,6,7,8:Job Process)
  <L 2 RCMD Set
    <A 1 RCMD>
    <L 1 RCMD Type Set
      <L 7 Job Select Set
        <A 40 PARENTLOT>
        <A 40 RFID>
        <A 40 EQPID>
        <A 4 PORTNO>
        <A 40 PPID>
        <A 5 CELLCNT>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F41 P HOST Command Send(HCS) (RCMD 9:Job(=PPID) Change Reserve)
  <L 2 RCMD Set
    <A 1 RCMD>
    <L 1 RCMD Type Set
      <L 4 Job Select Set
        <A 40 ASSYCODE>
        <A 40 PPID>
        <A 40 EQPID>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F41 P HOST Command Send(HCS) (RCMD 10:Equipment Command (Function Change))
  <L 3 RCMD Set
    <A 2 RCMD>
	<A 40 EQPID>
    <L 1 Function Change Set
      <L 4 Function Set
        <A 40 MODULEID>
        <A 2 EFID>
        <A 10 EFST>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F41 P HOST Command Send(HCS) (RCMD 11, 12, 13, 14:Equipment Command (Unit Machine Control))
  <L 3 RCMD Set
    <A 2 RCMD>
	<A 40 EQPID>
    <L 1 RCMD TYPE SET
      <L 4 Function Set
        <A 9 INTERLOCK>
        <A 40 MODULEID>
        <A 20 INTERLOCKID>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F41 P HOST Command Send(HCS) (RCMD 15:Equipment Command (Control Information))
  <L 3 RCMD Set
    <A 2 RCMD>
	<A 40 EQPID>
    <L 4 RCMD Attribute Set
      <L 2 Action Attribute Set
        <A 10 ACTIONTYPE>
        <A 20 ActionType>
      >
	  <L 2 Action Attribute Set
        <A 12 ACTIONDETAIL>
        <A 20 ActionDetail>
      >
	  <L 2 Action Attribute Set
        <A 6 ACTION>
        <A 20 Action>
      >
	  <L 2 Action Attribute Set
        <A 11 DESCRIPTION>
        <A 40 Description>
      >
    >
  >
>

<S2F41 P HOST Command Send(HCS) (RCMD 16 : Equipment Command (Unit Op-call Send))
  <L 3 RCMD Set
    <A 2 RCMD>
	<A 40 EQPID>
    <L 1 RCMD TYPE SET
      <L 4 Op-Call Set
        <A 6 OPCALL>
        <A 40 MODULEID>
        <A 20 OPCALLID>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F42 S HOST Command Acknowledge(HCA)
  <L 2 
    <A 2 RCMD>
    <A 1 HCACK>
  >
>

<S2F43 P HOST Command Send for job Process (HCS) (RCMD 21,22,23,24 : Cell Job Process)
  <L 2 RCMD Set
    <A 2 RCMD>
    <L 5 RCMD Attribute Set
      <L 2 Job Attribute Set
        <A 5 JOBID>
        <A 40 JobID>
      >
	  <L 2 Cell Attribute Set
        <A 6 CELLID>
        <A 40 CellID>
      >
	  <L 2 Product Attribute Set
        <A 9 PRODUCTID>
        <A 40 ProductID>
      >
	  <L 2 Step Attribute Set
        <A 6 STEPID>
        <A 40 StepID>
      >
	  <L 2 Action Attribute Set
        <A 10 ACTIONTYPE>
        <A 40 ActionType>
      >
    >
  >
>

<S2F43 P HOST Command Send for job Process (HCS) (RCMD 31,32 : Equipment Approve Process)
  <L 3 RCMD Set
    <A 2 RCMD>
	<A 40 EQPID>
    <L 5 RCMD Attribute Set
      <L 2 Approve Attribute Set
        <A 11 APPROVECODE>
        <A 20 ApproveCode>
      >
	  <L 2 Approve Attribute Set
        <A 11 Attribute Name>
        <A 20 Approve Info>
      >
	  <L 2 Approve Attribute Set
        <A 9 APPROVEID>
        <A 20 ApproveID>
      >
	  <L 2 Approve Attribute Set
        <A 5 BYWHO>
        <A 4 Bywho>
      >
	  <L 2 Approve Attribute Set
        <A 11 APPROVETEXT>
        <A 120 ApproveText>
      >
    >
  >
>

<S2F44 S HOST Command Acknowledge(HCA)
  <L 2 
    <A 3 RCMD>
    <A 1 HCACK>
  >
>

<S2F49 P HOST Command Send(AGV) (RCMD 1:OPCAL Send, RCMD 2:Interlock Send, RCMD 3:Job(=PPID)Select, RCMD 4~9:Job Proecess)
  <L 3 RCMD Set
    <A 3 RCMD>
	<A 40 EQPID>
    <L 1 RCMD Type Set
      <L 15 Job Select Set
        <A 4 PORTID>
        <A 20 TRSNAME>
        <A 40 JOBID>
        <A 20 JOBTYPE>
        <A 40 PRODUCTID>
		<A 40 STEPID>
		<A 20 SOURCELOC>
        <A 4 SOURCEPORTID>
        <A 20 FINALLOC>
        <A 4 FINALPORTID>
        <A 20 MIDLOC>
        <A 4 MIDPORTID>
        <A 20 ORIGINLOC>
		<A 4 PRIORITY>
        <A 40 DESCRIPTION>
      >
    >
  >
>

<S2F50 S HOST Command Acknowledge(AGV)
  <L 4
    <A 3 RCMD>
    <A 40 EQPID>
	<A 40 JOBID>
	<A 1 HCACK>
  >
>

<S2F141 P HOST Command Send(HCS) (RCMD 1: Equipment Command (OPCALL Send))
  <L 2 RCMD Set
    <A 2 RCMD>
    <L 1 RCMD Type Set
      <L 5 Opcall Set
        <A 6 OPCALL>
        <A 40 EQPID>
        <A 40 MODULEID>
        <A 20 OPCALLID>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F141 P HOST Command Send(HCS) (RCMD 2: Equipment Command (INTERLOCK Send))
  <L 2 RCMD Set
    <A 2 RCMD>
    <L 1 RCMD Type Set
      <L 5 Opcall Set
        <A 9 INTERLOCK>
        <A 40 EQPID>
        <A 40 MODULEID>
        <A 20 INTERLOCKID>
        <A 120 MESSAGE>
      >
    >
  >
>

<S2F141 P HOST Command Send(HCS) (RCMD 4~8: Equipment Process Command (Job Process))
  <L 2 RCMD Set
    <A 2 RCMD>
    <L 1 RCMD Type Set
      <L 8 Job Select Set
        <A 40 PARENTLOT>
        <A 40 RFID>
        <A 40 EQPID>
		<A 40 MODULEID>
        <A 4 PORTID>
        <A 40 PPID>
		<A 5 CELLCNT>
		<A 120 MESSAGE>
      >
    >
  >
>

<S2F142 S HOST Command Acknowledge(HCA)
  <L 4
    <A 2 RCMD>
    <A 40 EQPID>
	<A 40 MODULEID>
	<A 1 HCACK>
  >
>

<S2F101 P Process Job Download
  <L 4 Process Job Set
    <A 40 PROCESSJOB>
    <A 40 EQPID>
    <A 40 PRODUCTID>
    <A 10 CELLQTY>
  >
>

<S2F102 S Process Job Download Reply
  <A 1 ACKC2>
>

<S2F103 P Cell Information Download
  <L 4 Cell Info Set
    <A 40 EQPID>
    <A 40 CELLID>
    <A 40 PRODUCTID>
    <A 2 CELLINFORESULT>
  >
>

<S2F104 S Cell Information Download Reply
  <A 1 ACKC2>
>

<S2F105 P Code Information Download
  <L 3 Code Info set
    <A 40 EQPID>
    <A 1 CODETYPE>
    <L n Reason Code L
      <L 4 Reason Code Set
        <A 20 REASONCODE>
        <A 40 DESCRIPTION>
        <A 40 ENGLISHDESCRIPTION>
        <A 40 CHINESEDESCRIPTION>
      >
    >
  >
>

<S2F106 S Code Information Download Reply
  <A 1 ACKC2>
>

<S2F107 P BatchLot Information Download
  <L 6 BatchLot Info Set
    <A 40 EQPID>
    <A 40 PRODUCTID>
    <A 40 BATCHLOT>
    <A 5 BATCHLOTQTY>
    <A 10 REASONCODE>
    <A 120 DESCRIPTION>
  >
>

<S2F108 S BatchLot Information Download Reply
  <A 1 ACKC2>
>

<S2F109 P Slip Lot Information Download
  <L 10 SlipLot Info set
    <A 40 EQPID>
    <A 40 PRODUCTID>
    <A 40 SLIPID>
    <A 40 ITEM_1QTY>
    <L n The Numbers of Item#1
      <A 40 ITEM_1VALUE>
    >
    <A 40 ITEM_2QTY>
    <L n The Numbers of Item#2
      <A 40 ITEM_2VALUE>
    >
    <A 4 REPLYSTATUS>
    <A 10 REPLYCODE>
    <A 120 REPLYTEXT>
  >
>

<S2F110 S Slip Lot Information Download Reply
  <A 1 ACKC2>
>

<S3F0 N Abort Transaction
>

<S3F101 P Cassette Information Send
  <L 3 
    <A 40 EQPID>
    <L n Cassette L Count
      <L 12 Cassette Set
        <A 40 CASSETTEID>
        <A 20 CASSETTETYPE>
        <A 20 BATCHLOT>
        <A 5 BATCHLOTQTY>
        <A 40 PRODUCTID>
        <A 4 PRODUCT_TYPE>
        <A 4 PRODUCT_KIND>
        <A 40 PRODUCTSPEC>
        <A 40 PPID>
        <A 40 STEPID>
        <A 40 COMMENT>
        <L n The Numbers of Glass
          <A 40 CELLID>
        >
      >
    >
    <L n Cassette Specific
      <L 2 Cassette Item Set
        <A 40 ITEM_NAME>
        <A 80 ITEM_VALUE>
      >
    >
  >
>

<S3F102 S Cassette Information Send Acknowledge
  <A 1 ACKC3>
>

<S3F103 P Specific Validation Data Send
  <L 4 Reply Specific Validation Info Set
    <A 40 EQPID>
	<A 40 CARRIERID>
    <L 14 Specific Info Set
	  <A 200 UNIQUEID>
	  <A 20 UNIQUETYPE>
	  <A 40 PRODUCTID>
	  <A 40 PRODUCTSPEC>
	  <A 4 PRODUCT_TYPE>
	  <A 4 PRODUCT_KIND>
	  <A 40 PPID>
	  <A 40 STEPID>
	  <A 16 CELL_SIZE>
	  <A 16 CELL_THICKNEWW>
	  <A 2 CELLINFORESULT>
	  <A 10 INS_COUNT>
	  <A 40 comment>
      <L n Attribute Count
		<L 2 Msg Set
          <A 40 ITEM_NAME>
          <A 40 ITEM_VALUE>
		>
      >
    >
    <L 2 Reply Info Set
      <A 4 REPLYSTATUS>
      <A 120 REPLYTEXT>
    >
  >
>

<S3F104 S Specific Validation Data Reply
  <A 1 ACKC3>
>

<S3F105 P Material Information Send
  <L 2 
    <A 40 EQPID>
    <L n The Numbers of Requesting Materials
      <L 3 Material Set
        <L 9 Material Standard Information
          <A 40 MATERIALEQPID>
          <A 40 MATERIALBATCHID>
          <A 20 MATERIALCODE>
          <A 14 MATERIALUSEDATE>
          <A 14 MATERIALDISEASEDATE>
          <A 20 MATERIALMAKER>
          <A 4 MATERIALVALIDATIONFLAGE>
          <A 4 MATERIALDEFECTCODE>
          <A 40 COMMENT>
        >
        <L 11 Material Use Information
          <A 60 MATERIALID>
          <A 20 MATERIALTYPE>
          <A 1 MATERIALST>
          <A 1 MATERIALPORTID>
          <A 1 MATERIALSTATE>
          <A 10 MATERIALTOTOALQTY>
          <A 10 MATERIALUSEQTY>
          <A 10 MATERIALASSEMQTY>
          <A 10 MATERIALNGQTY>
          <A 10 MATERIALREMAINQTYQTY>
          <A 10 MATERIALPROCEUSEQTY>
        >
        <L 3 
          <A 4 REPLYSTATUS>
          <A 4 REPLYCODE>
          <A 120 REPLYTEXT>
        >
      >
    >
  >
>

<S3F106 S Material Information Send Reply
  <A 1 ACKC3>
>

<S3F107 P Tray Information Send
  <L 6 
    <A 40 EQPID>
    <A 40 PRODUCTID>
    <A 40 SLIPID>
    <A 40 ITEM_1QTY>
    <A 40 ITEM_2QTY>
    <A 40 DESCRIPTION>
  >
>

<S3F108 S Tray Information Send Reply
  <A 1 ACKC3>
>

<S3F109 P Cell Lot Information Send
  <L 2 
    <A 40 EQPID>
    <L n Cell Lot L Count
      <L 13 Cell Lot Set
        <A 40 CELLID>
        <A 40 CASSETTEID>
        <A 20 BATCHLOT>
        <A 40 PRODUCTID>
        <A 4 PRODUCT_TYPE>
        <A 4 PRODUCT_KIND>
        <A 40 PRODUCTSPEC>
        <A 40 STEPID>
        <A 20 PPID>
        <A 16 CELL_SIZE>
        <A 16 CELL_THICKNESS>
        <A 40 COMMENT>
        <L n Cell Specific
          <L 2 Item Set
            <A 40 ITEM_NAME>
            <A 80 ITEM_VALUE>
          >
        >
      >
    >
  >
>

<S3F110 S Cell Lot Information Send Reply
  <A 1 ACKC3>
>

<S3F111 P Label Information Send
	<L 3
		<A 40 EQPID>
		<A 10 OPTIONCODE>
		<L 6 Label Information Send
			<A 40 CELLID>
			<A 40 PRODUCTID>
			<A 200 LABELID>
			<A 4 REPLYSTATUS>
			<A 10 REPLYCODE>
			<A 120 REPLYTEXT>
		>
	>
>

<S3F112 S Label Information Send Reply
  <A 1 ACKC3>
>

<S3F113 P FPO Information Send
  <L 14 
    <A 40 EQPID>
    <A 40 CELLID>
    <A 40 PRODUCTID>
    <A 20 FPOID>
    <A 20 SFPOID>
    <A 4 FPO_SIZE>
    <A 4 FPO_QTY>
    <A 4 SFPO_SIZE>
    <A 4 SFPO_QTY>
    <A 4 SAMPLE_SIZE>
    <A 4 SAMPLE_QTY>
    <A 40 REPLYSTATUS>
    <A 120 REPLYTEXT>
    <L n 
      <L 2  
          <A 40 ITEM_NAME>
          <A 80 ITEM_VALUE>
      >
    >
  >
>

<S3F114 S Label Validation Send Reply
  <A 1 ACKC3>
>

<S3F115 P Carrier Information Send
  <L 2
    <A 40 EQPID>
	<L 10
	  <A 40 CARRIERID>
	  <A 4 CARRIERTYPE>
	  <A 40 CARRIERPPID>
	  <A 40 CARRIERPRODUCT>
	  <A 40 CARRIERSTEPID>
	  <A 4 CARRIER_S_COUNT>
	  <A 4 CARRIER_C_COUNT>
	  <A 4 PORTNO>
	  <L n
	    <L 3
		  <A 40 SUBCARRIERID>
		  <A 4 CELLQTY>
		  <L n
		    <L 4
			  <A 40 CELLID>
			  <A 4 LOCATIONNO>
			  <A 1 JUDGE>
			  <A 20 REASONCODE>
			>
		  >
		>
	  >
	  <L 2
	    <A 10 REPLYCODE>
		<A 120 REPLYTEXT>
	  >
	>
  >
>

<S3F116 S Carrier Information Send Reply
  <A 1 ACKC3>
>

<S3F215 P Checker L Information Request
  <A 40 EQPID>
>

<S3F216 S Checker L Information Reply
  <L 4 Checker L Info
    <A 1 ACKC3>
    <A 40 EQPID>
    <A 120 ERRORMESSAGE>
    <L n Checker L
      <L 2 Checker Set
        <A 40 CHECKERNAME>
        <A 40 CHECKERDESC>
      >
    >
  >
>

<S3F217 P Packing Information Request
  <L 4 Request Packing Info
    <A 40 EQPID>
    <A 40 SBPID>
    <A 40 CHECKERNAME>
    <A 20 CHIPMENTTYPE>
  >
>

<S3F218 S Packing Information Reply
  <L 6 Reply Packing Info
    <A 1 ACKC3>
    <A 40 EQPID>
    <L 8 Packing Info
      <A 120 ERRORMESSAGE>
      <A 40 SBPID>
      <A 40 CHECKERNAME>
      <A 20 SHIPMENTTYPE>
      <A 20 CUSTOMERID>
      <A 20 CELLSIZE>
      <A 40 ASSY_CODE>
      <A 20 PRODUCTQUANTITY>
    >
    <L 5 SBP Set
      <A 40 SBPLABELTYPE>
      <A 400 SBPLABELURL>
      <A 40 SBPUPPERWEIGHT>
      <A 40 SBPLOWERWEIGHT>
      <A 40 SBPREALWEIGHT>
    >
    <L 4 CARTON Data
      <A 40 CARTONID>
      <A 40 CARTONLABELTYPE>
      <A 400 CARTONLABELURL>
      <A 40 CARTONREALWEIGHT>
    >
    <L 4 ETC Label Data
      <A 1 REPAIRFLAG>
      <A 1 REWORKFLAG>
      <A 1 ETCLABELUSEYN>
      <A 400 ETCLABELURL>
    >
  >
>

<S5F0 N Abort Transaction
>

<S5F1 P Alarm Report Send
  <L 5 
    <A 40 EQPID>
    <A 1 ALST>
    <A 1 ALCD>
    <A 10 ALID>
    <A 120 ALTX>
  >
>

<S5F2 S Alarm Report Acknowledge
  <A 1 ACKC5>
>

<S5F3 P Enable/Disable Alarm Send
  <L 3 
    <A 1 ALED>
    <A 40 EQPID>
    <L n 
      <A 10 ALID>
    >
  >
>

<S5F4 S Enable/Disable Alarm acknowledge
  <A 1 ACKC5>
>

<S5F5 P TRS Alarm Report Send(TARS)
  <L 6
    <A 40 EQPID>
    <A 20 TRSNAME>
    <A 1 ALST>
    <A 1 ALCD>
    <A 10 ALID>
    <A 120 ALTX>
  >
>

<S5F6 S  TRS Alarm Report acknowledge
  <A 1 ACKC5>
>

<S5F11 P Alarm Report Send(ARS)
  <L 6
    <A 40 EQPID>
    <A 40 MODULEID>
    <A 1 ALST>
    <A 1 ALCD>
    <A 10 ALID>
    <A 120 ALTX>
  >
>

<S5F12 S  Alarm Report acknowledge
  <A 1 ACKC5>
>

<S5F13 P Enable/Disable Alarm Send
  <L 4
    <A 1 ALED>
    <A 40 EQPID>
    <A 40 MODULEID>
	<L n Count of ALID
	  <A 10 ALID>
	>
  >
>

<S5F14 S  Alarm Report acknowledge
  <A 1 ACKC5>
>

<S5F103 P Current Alarm L Request
  <L n 
    <A 40 EQPID>
  >
>

<S5F104 S Current Alarm L Reply
  <L n 
    <L 2 
      <A 40 EQPID>
      <L n 
        <L 3 
          <A 1 ALCD>
          <A 10 ALID>
          <A 120 ALTX>
        >
      >
    >
  >
>

<S5F113 P Current Alarm List Request(CALR)
  <L 2
    <A 40 EQPID>
	<L n Count of Unit
	  <A 40 MODULEID>
	>
  >
>

<S5F114 S  Current Alarm List Reply
  <L 2 
    <A 40 EQPID>
    <L n 
	  <L 2 
		<A 40 MODULEID>
		<L n 
          <L 3 
			<A 1 ALCD>
			<A 10 ALID>
			<A 120 ALTX>
		  >
        >
      >
    >
  >
>

<S6F0 N Abort Transaction
>

<S6F1 N Trace Data Send
  <L 5 
    <A 40 EQPID>
    <A 5 TRID>
    <A 5 SMPLN>
    <A 14 STIME>
    <L n 
      <L 2 
        <A 20 SVID>
        <A 200 SV>
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 101: Equipment Status Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 103>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 104>
        <L n 
          <L 4 
            <A 1 ALST>
            <A 1 ALCD>
            <A 10 ALID>
            <A 120 ALTX>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 102: Unit Status Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 111>
		<L 2
		  <A 40 MODULEID>
          <L 9 
			<A 1 AVILABILITYSTATE>
			<A 1 INTERLOCKSTATE>
			<A 1 MOVESTATE>
			<A 1 RUNSTATE>
			<A 1 FRONTSTATE>
			<A 1 REARSTATE>
			<A 1 PP_SPLSTATE>
			<A 20 REASONCODE>
			<A 40 DESCRIPTION>
		  >
        >
      >
	  <L 2 
        <A 3 112>
		<L 2
		  <A 40 MODULEID>
          <L 9 
			<A 1 AVILABILITYSTATE>
			<A 1 INTERLOCKSTATE>
			<A 1 MOVESTATE>
			<A 1 RUNSTATE>
			<A 1 FRONTSTATE>
			<A 1 REARSTATE>
			<A 1 PP_SPLSTATE>
			<A 20 REASONCODE>
			<A 40 DESCRIPTION>
		  >
        >
      >
      <L 2 
        <A 3 104>
        <L n 
          <L 4 
            <A 1 ALST>
            <A 1 ALCD>
            <A 10 ALID>
            <A 120 ALTX>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 103: AGV Status Change)
  <L 3
    <A 4 DATAID>
    <A 3 CEID>
    <L 4
      <L 2
        <A 3 100>
        <L 2
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2
        <A 3 107>
        <L 8
          <A 20 AGVNAME1>
          <A 1 AGVAVILABILITYSTATE1>
          <A 1 AGVINTERLOCKSTATE1>
          <A 1 AGVRECHARGESTATE1>
          <A 1 AGVMOVESTATE1>
          <A 1 AGVRUNSTATE1>
          <A 20 AGVREASONCODE1>
          <A 40 AGVDESCRIPTION1>
        >
      >
      <L 2
        <A 3 108>
        <L 8
          <A 20 AGVNAME2>
          <A 1 AGVAVILABILITYSTATE2>
          <A 1 AGVINTERLOCKSTATE2>
          <A 1 AGVRECHARGESTATE2>
          <A 1 AGVMOVESTATE2>
          <A 1 AGVRUNSTATE2>
          <A 20 AGVREASONCODE2>
          <A 40 AGVDESCRIPTION2>
        >
      >
      <L 2
        <A 3 109>
        <L n
          <L 5
            <A 20 AGVNAME>
            <A 1 ALST>
            <A 1 ALCD>
            <A 10 ALID>
            <A 120 ALTX>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 104,105,106: Control Status Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 107: PPID Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 302>
        <L 3 
          <A 40 PPID>
          <A 1 PPID_TYPE>
          <A 40 OLD_PPID>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 108: PPID Parameter Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 7 
          <A 1 MAINTSTATE>
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 312>
        <L 2 
          <A 40 PPID>
          <A 1 PPIDST>
        >
      >
      <L 2 
        <A 3 303>
        <L n 
          <L 2 
            <A 20 PARAMETERNAME>
            <A 40 PARAMETERVALUE>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 109: Equipment Configuration File Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 311>
        <L 4 
          <A 40 PRODUCTID>
          <A 20 ACTIONTYPE>
          <A 20 ACTIONRESULT>
          <L n 
			<L 5 
              <A 20 FILETYPE>
              <A 60 FILENAME>
			  <A 100 FILEPATHE>
			  <A 40 LOCALCHECKSUM>
			  <A 40 CURRENTCHECKSUM>
			>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 111: Equipment Function Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 314>
        <L n 
          <L 4 
		    <A 4 BYWHO>
		    <A 2 EFID>
		    <A 10 NEWEFST>
		    <A 10 OLDEFST>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 117: PPID Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 313>
        <L 2 
		  <A 40 MODULEID>
          <L 3 
		    <A 40 PPID>
		    <A 1 PPID_TYPE>
		    <A 40 OLD_PPID>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 200,201: Material Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 200>
        <L n 
          <L 5 
            <A 60 MATERIALID>
            <A 20 MATERIALTYPE>
            <A 1 MATERIALST>
            <A 1 MATERIALPORTID>
            <A 20 MATERIALUSAGE>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 211~225: Material Process Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 201>
        <L n 
          <L 17 
            <A 40 EQPMATERIALBATCHID>
            <A 40 EQPMATERIALBATCHNAME>
            <A 60 EQPMATERIALID>
            <A 20 EQPMATERIALTYPE>
            <A 1 EQPMATERIALST>
            <A 1 EQPMATERIALPORTID>
            <A 1 EQPMATERIALSTATE>
            <A 10 EQPMATERIALTOTALQTY>
            <A 10 EQPMATERIALUSEQTY>
            <A 10 EQPMATERIALASSEMQTY>
            <A 10 EQPMATERIALNGQTY>
            <A 10 EQPMATERIALREMAINQTYQTY>
            <A 10 EQPMATERIALPRODUCTQTY>
            <A 10 EQPMATERIALPROCEUSEQTY>
            <A 10 EQPMATERIALPROCEASSEMQTY>
            <A 10 EQPMATERIALPROCNGQTY>
            <A 10 EQPMATERIALSUPPLYREQUESTQTY>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 250~253: Carrier Status Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 305>
        <L 5 
          <A 4 PORTNO>
          <A 1 PORTAVAILABLESTATE>
          <A 1 PORTACCESSMODE>
          <A 1 PORTTRANSFERSTATE>
          <A 1 PORTPROCESSINGSTATE>
        >
      >
      <L 2 
        <A 3 250>
        <L 4 
          <A 40 PARENTLOT>
          <A 40 RFID>
          <A 1 PORTNO_1>
          <A 40 PPID>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 256~262: Carrier Status Change)
  <L 3
	<A 4 DATAID>
	<A 3 CEID>
	<L 3
	  <L 2
	    <A 3 100>
		<L 2
		  <A 40 EQPID>
		  <A 1 CRST>
		>
	  >
	  <L 2
	    <A 3 309>
		<L 8
		  <A 40 CARRIERID>
		  <A 4 CARRIERTYPE>
		  <A 40 CARRIERPPID>
		  <A 40 CARRIERPRODUCT>
		  <A 40 CARRIERSTEPID>
		  <A 4 CARRIER_S_COUNT>
		  <A 4 CARRIER_C_COUNT>
		  <A 4 PORTNO>
		>
	  >
	  <L 2
	    <A 3 310>
		<L n
		  <L 3
		    <A 40 SUBCARRIERID>
			<A 4 CELLQTY>
			<L n
			  <L 4
			    <A 200 CELLID>
				<A 4 LOCATIONNO>
				<A 1 JUDGE>
				<A 20 REASONCODE>
			  >			  
			>
		  >
		>
	  >
	>
  >
>

<S6F11 P Event Report Send(CEID 254,255: Port Status Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 305>
        <L 5 
          <A 4 PORTNO>
          <A 1 PORTAVAILABLESTATE>
          <A 1 PORTACCESSMODE>
          <A 1 PORTTRANSFERSTATE>
          <A 1 PORTPROCESSINGSTATE>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 301~306: Process Job)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 5 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 200>
        <L n 
          <L 5 
            <A 60 MATERIALID>
            <A 20 MATERIALTYPE>
            <A 1 MATERIALST>
            <A 1 MATERIALPORTID>
            <A 20 MATERIALUSAGE>
          >
        >
      >
      <L 2 
        <A 3 305>
        <L 5 
          <A 4 PORTNO>
          <A 1 PORTAVAILABLESTATE>
          <A 1 PORTACCESSMODE>
          <A 1 PORTTRANSFERSTATE>
          <A 1 PORTPROCESSINGSTATE>
        >
      >
      <L 2 
        <A 3 306>
        <L 5 
          <A 40 PARENTLOT>
          <A 40 RFID>
          <A 4 PORTNO_1>
          <A 10 PLANQTY>
          <A 10 PROCESSEDQTY>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 311~318: AGV Process Job)
  <L 3
    <A 4 DATAID>
    <A 3 CEID>
    <L 2
      <L 2
        <A 3 100>
        <L 2
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2
        <A 3 304>
        <L 15
          <A 40 EQPID1>
          <A 4 PORTID1>
          <A 20 TRSNAME>
          <A 40 JOBID>
          <A 20 JOBTYPE>
          <A 20 CURRENTLOC>
          <A 20 SOURCELOC>
          <A 4 SOURCEPORTID>
          <A 20 FINALLOC>
          <A 4 FINALPORTID>
          <A 20 MIDLOC>
          <A 4 MIDPORTID>
          <A 20 ORIGINLOC>
		  <A 4 PRIORITY>
          <A 40 DESCRIPTION>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 350~356: Cassette Status Change)
  <L 3
    <A 4 DATAID>
    <A 3 CEID>
    <L 3
      <L 2
        <A 3 100>
        <L 2
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2
        <A 3 307>
        <L 5
          <A 4 PORTID>
          <A 1 PORTAVAILABLESTATE>
          <A 1 PORTACCESSMODE>
          <A 1 PORTTRANSFERSTATE>
          <A 1 PORTPROCESSINGSTATE>
        >
      >
      <L 2
        <A 3 251>
        <L 2
          <A 40 JOBID>
          <A 20 JOBTYPE>
        >
      >      
    >
  >
>

<S6F11 P Event Report Send(CEID 390~396: Cassette Status Change)
  <L 3
    <A 4 DATAID>
    <A 3 CEID>
    <L 3
      <L 2
        <A 3 100>
        <L 2
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2
        <A 3 307>
        <L 2
		  <A 40 MODULEID>
		  <L 5
			<A 4 PORTID>
			<A 1 PORTAVAILABLESTATE>
			<A 1 PORTACCESSMODE>
			<A 1 PORTTRANSFERSTATE>
			<A 1 PORTPROCESSINGSTATE>
		  >
        >
      >
      <L 2
        <A 3 251>
        <L 4
          <A 20 TRSID>
          <A 20 OBJECTTYPE>
		  <A 40 PRODUCTID>
		  <A 20 TRAYTYPE>
        >
      >      
    >
  >
>

<S6F11 P Event Report Send(CEID 401: Cell Process Start)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 5 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 301>
        <L 3 
          <A 40 PROCESSJOB>
          <A 10 PLANQTY>
          <A 10 PROCESSEDQTY>
        >
      >
      <L 2 
        <A 3 400>
        <L 2 
          <A 10 READERID>
          <A 1 READERRESULTCODE>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 402: Cell Process Complete)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 8 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 301>
        <L 3 
          <A 40 PROCESSJOB>
          <A 10 PLANQTY>
          <A 10 PROCESSEDQTY>
        >
      >
      <L 2 
        <A 3 400>
        <L 2 
          <A 10 READERID>
          <A 1 READERRESULTCODE>
        >
      >
      <L 2 
        <A 3 200>
        <L n 
          <L 5 
            <A 60 MATERIALID>
            <A 20 MATERIALTYPE>
            <A 1 MATERIALST>
            <A 1 MATERIALPORTID>
            <A 20 MATERIALUSAGE>
          >
        >
      >
      <L 2 
        <A 3 600>
        <L n 
          <L 2 
            <A 20 DVNAME>
            <A 40 DV>
          >
        >
      >
      <L 2 
        <A 3 500>
        <L 4 
          <A 20 OPERATORID>
          <A 1 JUDGE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 403: Normal Data Collection)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 600>
        <L n 
          <L 2 
            <A 40 DVNAME>
            <A 40 DV>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 404: Cassette Unit Process)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 303>
        <L 8 
          <A 40 PRODUCTID>
          <A 40 PPID>
          <A 40 CASSETTEID>
          <A 20 CASSETTETYPE>
          <A 40 ITEMCOUNT1>
          <A 40 ITEMCOUNT2>
          <A 40 FROMUNITID>
          <A 40 TOUNITID>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 405: EQP Specific Step)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 601>
        <L 2 
          <A 20 ITEMNAME>
          <A 40 ITEMVALUE>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 406: Cell Process End)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 8 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 301>
        <L 3 
          <A 40 PROCESSJOB>
          <A 10 PLANQTY>
          <A 10 PROCESSEDQTY>
        >
      >
      <L 2 
        <A 3 400>
        <L 2 
          <A 10 READERID>
          <A 1 READERRESULTCODE>
        >
      >
      <L 2 
        <A 3 201>
        <L n 
          <L 17 
            <A 40 EQPMATERIALBATCHID>
            <A 40 EQPMATERIALBATCHNAME>
            <A 60 EQPMATERIALID>
            <A 20 EQPMATERIALTYPE>
            <A 1 EQPMATERIALST>
            <A 1 EQPMATERIALPORTID>
            <A 1 EQPMATERIALTSTATE>
            <A 10 EQPMATERIALTOTALQTY>
            <A 10 EQPMATERIALUSEQTY>
            <A 10 EQPMATERIALASSEMQTY>
            <A 10 EQPMATERIALNGQTY>
            <A 10 EQPMATERIALREMAINQTYQTY>
            <A 10 EQPMATERIALPRODUCTQTY>
            <A 10 EQPMATERIALPROCEUSEQTY>
            <A 10 EQPMATERIALPROCEASSEMQTY>
            <A 10 EQPMATERIALPROCNGQTY>
            <A 10 EQPMATERIALSUPPLYREQUESTQTY>
          >
        >
      >
      <L 2 
        <A 3 600>
        <L n 
          <L 2 
            <A 40 DVNAME>
            <A 40 DV>
          >
        >
      >
      <L 2 
        <A 3 501>
        <L 6 
          <A 20 OPERATORID1>
          <A 20 OPERATORID2>
          <A 20 OPERATORID3>
          <A 1 JUDGE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 411: Pair Cell Process Start)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 308>
        <L 5 
          <A 200 PAIRCELLID>
          <A 10 PAIRTYPE>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 400>
        <L 2 
          <A 10 READRID>
          <A 1 READERRESULTCODE>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 412: Pair Cell Process End)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 5 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 308>
        <L 5 
          <A 200 PAIRCELLID>
          <A 10 PAIRTYPE>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 400>
        <L 2 
          <A 10 READERID>
          <A 1 READERRESULTCODE>
        >
      >
      <L 2 
        <A 3 600>
        <L n
		  <L 2
			<A 40 DVNAME>
			<A 40 DV>
          >
        >
      >
      <L 2 
        <A 3 500>
        <L 4 
          <A 20 OPERATORID>
          <A 1 JUDGE>
		  <A 20 REASONCODE>
		  <A 40 DESCRIPTION>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 501: OPCALL Confirm)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 700>
        <L n The Number of OPCALL
          <L 2 OPCALL Set
            <A 20 OPCALLID>
            <A 120 MESSAGE>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 502: INTERLOCK Confirm)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 701>
        <L n The Number of Interlock
          <L 2 Interlock Set
            <A 20 INTERLOCKID>
            <A 120 MESSAGE>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 511: TRS OPCALL Confirm)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 107>
        <L 8 
          <A 1 TRSNAME1>
          <A 1 TRSAVAILABILITYSTATE1>
          <A 1 TRSINTERLOCKSTATE1>
          <A 1 TRSRECHARGESTATE1>
          <A 1 TRSMOVESTATE1>
          <A 1 TRSRUNSTATE1>
          <A 1 TRSREASONCODE1>
          <A 20 TRSDESCRIPTION1>
        >
      >
      <L 2 
        <A 3 700>
        <L n
		  <L 2
			<A 20 OPCALLID>
			<A 120 MESSAGE>
		  >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 512: TRS INTERLOCK Confirm)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 107>
        <L 8 
          <A 1 TRSNAME1>
          <A 1 TRSAVAILABILITYSTATE1>
          <A 1 TRSINTERLOCKSTATE1>
          <A 1 TRSRECHARGESTATE1>
          <A 1 TRSMOVESTATE1>
          <A 1 TRSRUNSTATE1>
          <A 1 TRSREASONCODE1>
          <A 20 TRSDESCRIPTION1>
        >
      >
      <L 2 
        <A 3 701>
        <L n
		  <L 2
			<A 20 INTERLOCKID>
			<A 120 MESSAGE>
		  >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 513: Unit OPCALL Confirm)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 102>
		<L 2
		  <A 40 MODULEID>
		  <L 9 
		    <A 1 AVILABILITYSTATE1>
		    <A 1 INTERLOCKSTATE1>
		    <A 1 MOVESTATE1>
		    <A 1 RUNSTATE1>
		    <A 1 FRONTSTATE1>
		    <A 1 REARSTATE1>
		    <A 1 PP_SPLSTATE1>
		    <A 20 REASONCODE1>
		    <A 40 DESCRIPTION1>
		  >
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 700>
        <L n The Number of OPCALL
          <L 2 OPCALL Set
            <A 20 OPCALLID>
            <A 120 MESSAGE>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 514: Unit INTERLOCK Confirm)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 4 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 102>
		<L 2
		  <A 40 MODULEID>
		  <L 9 
		    <A 1 AVILABILITYSTATE1>
		    <A 1 INTERLOCKSTATE1>
		    <A 1 MOVESTATE1>
		    <A 1 RUNSTATE1>
		    <A 1 FRONTSTATE1>
		    <A 1 REARSTATE1>
		    <A 1 PP_SPLSTATE1>
		    <A 20 REASONCODE1>
		    <A 40 DESCRIPTION1>
		  >
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 701>
        <L n The Number of Interlock
          <L 2 Interlock Set
            <A 20 INTERLOCKID>
            <A 120 MESSAGE>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 601: Reader Result)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 1 
      <L 5 
        <A 3 800>
        <A 40 EQPID>
        <A 40 CELLID>
        <A 10 READERID>
        <A 1 READERRESULTCODE>
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 602: Start Cell Lot)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 1 
      <L 5 
        <A 3 801>
        <A 40 EQPID>
        <A 10 READERID>
        <A 1 READERRESULTCODE>
        <L n 
          <L 2 
            <A 40 CELLID>
            <A 40 PARENTLOT>
          >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 603: Equipment Status Change By User)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 802>
        <L 2 
          <A 40 EQPID>
          <A 10 DATA_TYPE>
        >
      >
      <L 2 
        <A 3 803>
        <L 2 
          <A 20 ADDRESS>
          <A 20 VALUE>
        >
      >
      <L 2 
        <A 3 804>
        <L 2 
          <A 20 LOSSDISPLAY>
          <A 20 LOSS>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 604: RFID Reader Result)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 1 
      <L 5
		<A 3 800>
        <A 40 EQPID>
        <A 40 RFID>
        <A 40 READERID>
        <A 1 READERRESULTCODE>
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 605: RFID Reader Result)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 1 
      <L 4
		<A 3 810>
        <A 40 EQPID>
        <A 60 MATERIALID>
        <A 1 MATERIALPORTID>
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 606: Equipment Loss Code Report)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 806>
        <L 2 
          <A 20 LOSSCODE>
          <A 40 LOSSDESCRIPTION>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 607: Operator Infomation Report)
  <L 3
    <A 4 DATAID>
    <A 3 CEID>
    <L 2
      <L 2
        <A 3 105>
        <L 3
          <A 40 EQPID>
          <A 10 OPTIONINFO>
          <A 40 COMMENT>
        >
      >
      <L 2
        <A 3 106>
        <L 2
          <A 20 OPERATORID>
          <A 40 PASSWORD>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 608: Job Infomation Report)
  <L 3
    <A 4 DATAID>
    <A 3 CEID>
    <L 2
      <L 2
        <A 3 100>
        <L 2
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2
        <A 3 807>
        <L 6
          <A 40 FINALEQPID>
          <A 40 JOBID>
          <A 20 JOBTYPE>
          <A 1 READERID>
          <A 1 READERRESULT>
          <A 20 OPERID>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 609: Inspection Result Report)
  <L 3
    <A 4 DATAID>
    <A 3 CEID>
    <L 2
      <L 2
        <A 3 100>
        <L 2
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2
        <A 3 808>
        <L 8
		  <A 20 PROCESSNAME>
          <A 40 CELLID>
          <A 4 PROCESSFLAG>
		  <A 1 JUDGE>
		  <A 20 REASONCODE>
          <A 20 OPERID>
		  <A 200 SENDUNIQUEINFO>
		  <A 200 REVUNIQUEINFO>
		>
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 615: Material ID Reader Result)
  <L 3
    <A 4 DATAID>
    <A 3 CEID>
    <L 1
      <L 5
        <A 3 811>
		<A 40 EQPID>
		<A 40 MODULEID>
		<A 60 MATERIALID>
		<A 4 MATERIALPORTID>
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 616: Equipment Loss Code Report)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 102>
		<L 2
		  <A 40 MODULEID>
          <L 9 
			<A 1 AVILABILITYSTATE>
			<A 1 INTERLOCKSTATE>
			<A 1 MOVESTATE>
			<A 1 RUNSTATE>
			<A 1 FRONTSTATE>
			<A 1 REARSTATE>
			<A 1 PP_SPLSTATE>
			<A 20 REASONCODE>
			<A 40 DESCRIPTION>
		  >
        >
      >
      <L 2 
        <A 3 806>
        <L 2 
          <A 20 LOSSCODE>
          <A 40 LOSSDESCRIPTION>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 701~793: Ept Process)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 5 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 301>
        <L 3 
          <A 40 PROCESSJOB>
          <A 10 PLANQTY>
          <A 10 PROCESSEDQTY>
        >
      >
      <L 2 
        <A 3 400>
        <L 2 
          <A 10 READERID>
          <A 1 READERRESULTCODE>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 801~807: Packing Process)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 802>
        <L 6 
          <A 40 SBPID>
          <A 40 SBPREALWEIGHT>
          <A 40 CARTONID>
          <A 40 CARTONREALWEIGHT>
          <A 40 CHECKERNAME>
          <A 20 ERRORMESSAGE>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 811~813: Packing Job Process)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 5 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 304>
        <L 4 
          <A 40 SBPID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 301>
        <L 3 
          <A 40 PROCESSJOB>
          <A 10 PLANQTY>
          <A 10 PROCESSEDQTY>
        >
      >
      <L 2 
        <A 3 400>
        <L 2 
          <A 10 READERID>
          <A 1 READERRESULTCODE>
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 900,901: Unit Material Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 200>
        <L 2 
          <A 40 MODULEID>
		  <L 5
            <A 60 MATERIALID>
            <A 20 MATERIALTYPE>
            <A 1 MATERIALST>
            <A 4 MATERIALPORTID>
            <A 20 MATERIALUSAGE>
		  >
		>
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 911~925: Material Process Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 5 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 300>
        <L 4 
          <A 40 CELLID>
          <A 40 PPID>
          <A 40 PRODUCTID>
          <A 40 STEPID>
        >
      >
      <L 2 
        <A 3 201>
        <L 2 
          <A 40 MODULEID>
		  <L n
			<L 17
              <A 40 EQPMATERIALBATCHID>
              <A 40 EQPMATERIALBATCHNAME>
              <A 60 EQPMATERIALID>
			  <A 20 EQPMATERIALTYPE>
			  <A 1 EQPMATERIALST>
			  <A 4 EQPMATERIALPORTID>
			  <A 1 EQPMATERIALSTATE>
			  <A 10 EQPMATERIALTOTALQTY>
			  <A 10 EQPMATERIALUSEQTY>
			  <A 10 EQPMATERIALASSEMQTY>
			  <A 10 EQPMATERIALNGQTY>
			  <A 10 EQPMATERIALREMAINQTY>
			  <A 10 EQPMATERIALPRODUCTQTY>
			  <A 10 EQPMATERIALPROCUSEQTY>
			  <A 10 EQPMATERIALPROCASSEMQTY>
			  <A 10 EQPMATERIALPROCNGQTY>
			  <A 10 EQPMATERIALSUPPLYREQUESTQTY>
			>
		  >
        >
      >
    >
  >
>

<S6F11 P Event Report Send(CEID 954,955: Port State Change)
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 2 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 305>
        <L 2 
          <A 40 MODULEID>
		  <L 5
            <A 4 PORTID>
            <A 1 PORTAVAILABLESTATE>
            <A 1 PORTACCESSMODE>
            <A 1 PORTTRANSFERSTATE>
            <A 1 PORTPROCESSINGSTATE>
		  >
		>
      >
    >
  >
>

<S6F12 S Event Report Acknowledge
  <A 1 ACKC6>
>

<S6F201 P Label Information Request
	<L 3
		<A 40 EQPID>
		<A 10 OPTIONCODE>
		<L 4
			<A 40 CELLID>
			<A 40 PRODUCTID>
			<A 80 OPTIONINFO>
			<A 200 LABELID>
		>
	>
>

<S6F202 S Label Information Request Reply
  <A 1 ACKC6>
>

<S6F203 P Specific Validation Request
	<L 3
		<A 40 EQPID>
		<A 10 OPTIONCODE>
		<L 2
			<A 200 UNIQUEID>
			<A 80 OPTIONINFO>
		>
	>
>

<S6F204 S Specific Validation Request Reply
  <A 1 ACKC6>
>

<S6F205 P Cell Lot Information Request
  <L 3 
    <A 40 EQPID>
    <A 10 OPTIONCODE>
    <L n 
      <A 40 CELLID>
    >
  >
>

<S6F206 S Cell Lot Information Reply
  <A 1 ACKC6>
>

<S6F207 P Cell Information Request
  <L 2 
    <A 40 EQPID>
    <A 40 CELLID>
  >
>

<S6F208 S Cell Information Reply
  <A 1 ACKC6>
>

<S6F209 P Code Information Request
  <L 2 
    <A 40 EQPID>
    <A 40 CODETYPE>
  >
>

<S6F210 S Code Information Reply
  <A 1 ACKC6>
>

<S6F211 P Batch Information Request
  <L 2 
    <A 40 EQPID>
    <A 40 BATCHLOT>
  >
>

<S6F212 S Batch Information Reply
  <A 1 ACKC6>
>

<S6F213 P SlipLot Information Request
  <L 6 
    <A 40 EQPID>
    <A 40 SLIPID>
    <A 20 SLIPTYPE>
    <A 4 PORTNO>
    <A 4 BYWHO>
    <A 20 OPERATORID>
  >
>

<S6F214 S SlipLot Information Reply
  <A 1 ACKC6>
>

<S6F215 P Tray Packing Job Process End
  <L 3 
    <A 4 DATAID>
    <A 3 CEID>
    <L 3 
      <L 2 
        <A 3 100>
        <L 2 
          <A 40 EQPID>
          <A 1 CRST>
        >
      >
      <L 2 
        <A 3 101>
        <L 9 
          <A 1 AVILABILITYSTATE>
          <A 1 INTERLOCKSTATE>
          <A 1 MOVESTATE>
          <A 1 RUNSTATE>
          <A 1 FRONTSTATE>
          <A 1 REARSTATE>
          <A 1 PP_SPLSTATE>
          <A 20 REASONCODE>
          <A 40 DESCRIPTION>
        >
      >
      <L 2 
        <A 3 303>
        <L 10 
          <A 40 PACKEQPID>
          <A 40 PRODUCTID>
          <A 40 PPID>
          <A 40 LABELID>
          <A 4 BYWHO>
          <A 20 OPERATORID>
          <A 40 ITEM_1COUNT>
          <L n 
            <A 40 ITEM_1VALUE>
          >
          <A 40 ITEM_2COUNT>
          <L n 
            <A 40 ITEMVALUE>
          >
        >
      >
    >
  >
>

<S6F216 S Tray Packing Job Process End Reply
  <L 2 
    <A 40 EQPID>
    <L n 
      <L 3 
        <L 4 
          <A 40 PACKEQPID>
          <A 40 PRODUCTID>
          <A 40 PPID>
          <A 40 COMMENT>
        >
        <L 5 
          <A 40 LABELID>
          <A 14 LABELCREATETIME>
          <A 40 ITEM_1COUNT>
          <A 40 ITEM_2COUNT>
          <A 40 DESCRIPTION>
        >
        <L 3 
          <A 4 REPLYSTATUS>
          <A 10 REPLYCODE>
          <A 120 REPLYTEXT>
        >
      >
    >
  >
>

<S6F217 P FPO Create Request
  <L 14 
    <A 40 EQPID>
    <A 200 CELLID>
    <A 40 PRODUCTID>
    <A 20 FPOID>
    <A 20 SFPOID>
    <A 4 SAMPLEQTY>
    <A 10 FPO_TYPE>
    <A 1 SHIFTINFO>
    <A 20 OPERATORID1>
    <A 20 OPERATORID2>
    <A 1 FPO_COMP>
    <A 1 JUDGE>
    <A 20 REASONCODE>
    <L n 
      <L 2  
          <A 40 ITEM_NAME>
          <A 80 ITEM_VALUE>
      >
    >
  >
>

<S6F218 S FPO Request Reply
  <A 1 ACKC6>
>

<S7F0 N Abort Transaction
>

<S7F23 P Formatted Process Program Send
  <L 4 
    <A 40 EQPID>
    <A 40 PPID>
    <A 1 PPID_TYPE>
    <L n 
      <L 2 
        <A 3 CCODE>
        <L n 
          <L 2 
            <A 40 PARAMETERNAME>
            <A 40 PARAMETERVALUE>
          >
        >
      >
    >
  >
>

<S7F24 S Formatted Process Program Acknowledge
  <A 1 ACKC7>
>

<S7F25 P Formatted Process Program Request
  <L 3 
    <A 40 EQPID>
    <A 40 PPID>
    <A 1 PPID_TYPE>
  >
>

<S7F26 S Formatted Process Program Data
  <L 7 
    <A 40 EQPID>
    <A 40 PPID>
    <A 1 PPID_TYPE>
    <A 20 MDLN>
    <A 6 SOFTREV>
    <A 14 CIMTIME>
    <L 1 
      <L 2 
        <A 3 CCODE>
        <L n 
          <L 2 
            <A 40 PPARAMNAME>
            <A 40 PPARAMVALUE>
          >
        >
      >
    >
  >
>

<S7F101 P Current Equipment PPID L Request
  <L 2 
    <A 40 EQPID>
    <A 1 PPID_TYPE>
  >
>

<S7F102 S Current Equipment PPID Data
  <L 3 
    <A 40 EQPID>
    <A 1 PPID_TYPE>
    <L n 
      <A 40 PPID>
    >
  >
>

<S7F107 P PPID Create/Delete Report, PP Body Modify Report
  <L 5 
    <A 1 MODE>
    <A 40 EQPID>
    <A 40 PPID>
    <A 1 PPID_TYPE>
    <L n 
      <L 2 
        <A 3 CCODE>
        <L n 
          <L 2 
            <A 40 P_PARM_NAME>
            <A 40 P_PARM>
          >
        >
      >
    >
  >
>

<S7F108 S PPID Create/Delete Report, PP Body Modify Report
  <A 1 ACKC7>
>

<S7F109 P Current Running Equipment PPID Request
  <L 2 
    <A 40 EQPID>
    <A 1 PPID_TYPE>
  >
>

<S7F110 S Current Running Equipment PPID Request
  <L 2 
    <A 1 ACKC7>
    <L 3 
      <A 40 EQPID>
      <A 40 PPID>
      <A 1 PPID_TYPE>
    >
  >
>

<S7F123 P Formatted Process Program Send
  <L 5 
    <A 40 EQPID>
    <A 40 MODULEID>
	<A 40 PPID>
	<A 1 PPID_TYPE>
	<L n
	  <L 2
        <A 3 CCODE>
		<L n
		  <L 2
			<A 40 PARAMETERNAME>
			<A 40 PARAMETERVALUE>
		  >
		>
	  >
    >
  >
>

<S7F124 S Formatted Process Program Send
  <A 1 ACKC7>
>

<S7F125 P Formatted Process Program Send
  <L 4 
    <A 40 EQPID>
    <A 40 MODULEID>
	<A 40 PPID>
	<A 1 PPID_TYPE>
  >
>

<S7F126 S Formatted Process Program Send
  <L 8 
    <A 40 EQPID>
	<A 40 MODULEID>
	<A 40 PPID>
	<A 1 PPID_TYPE>
	<A 20 MDLN>
	<A 6 SOFTREV>
	<A 14 CIMTIME>
    <L 1 
	  <L 2 
      <A 3 CCODE>
	    <L n
          <A 40 PPRAMNAME>
          <A 40 PPARAMVALUE>
		>
	  >
    >
  >
>

<S8F0 N Abort Transaction
>

<S8F1 P Inquiry For Specially Designed Multi-Use Set
  <L 2 
    <A 40 EQPID>
    <L n 
      <L 2 
        <A 10 DATA_TYPE>
        <L 3 
          <A 40 ITEM_NAME>
          <A 100 ITEM_VALUE>
          <A 20 REFERENCE>
        >
      >
    >
  >
>

<S8F2 S Inquiry For Specially Designed Multi-Use Set Data
  <L 2 
    <A 1 ACKC8>
    <L n 
      <L 3 
        <A 10 DATA_TYPE>
        <L 3 
          <A 40 ITEM_NAME>
          <A 100 ITEM_VALUE>
          <A 20 REFERENCE>
        >
        <A 1 EAC>
      >
    >
  >
>

<S8F3 P Inquiry Form Multi-Use Set
  <L 2 
    <A 40 EQPID>
    <L n 
      <L 2 
        <A 10 DATA_TYPE>
        <L n 
          <A 40 ITEM_NAME>
        >
      >
    >
  >
>

<S8F4 S Inquiry Form Multi-Use Set Data
  <L 2 
    <A 1 ACKC8>
    <L n 
      <L 3 
        <A 10 DATA_TYPE>
        <L 3 
          <A 40 ITEM_NAME>
          <A 100 ITEM_VALUE>
          <A 20 REFERENCE>
        >
        <A 1 EAC>
      >
    >
  >
>

<S9F13 N Conversation Time Out
  <L 2 
    <A 6 MEXP>
    <A 64 EDID>
  >
>

<S10F0 N Abort Transaction
>

<S10F1 P Terminal Request
  <L 3 
    <A 40 EQPID>
    <A 2 TID>
    <A 120 TEXT>
  >
>

<S10F2 S Terminal Request Acknowledge
  <A 1 ACKC10>
>

<S10F5 P Terminal Display, Multi-Block
  <L 3 
    <A 40 EQPID>
    <A 2 TID>
    <L n 
      <A 120 TEXT>
    >
  >
>

<S10F6 S Terminal Display, Multi-Block Reply
  <A 1 ACKC10>
>

<S14F1 P GetAttr Request
  <L 4
    <A 40 EQPID>
    <A 20 OBJTYPE>
    <A 20 OBJID>
    <A 20 COMMENT>
  >
>

<S14F2 S GetAttr Data
  <L 6
    <A 40 EQPID>
    <A 20 OBJTYPE>
    <A 20 OBJID>
    <A 20 COMMENT>
    <L n
      <L 2
        <A 40 ATTRID>
        <A 40 ATTRDATA>
       >
    >
    <L 2
        <A 10 REPLYCODE>
        <A 120 REPLYTEXT>
    >
  >
>



