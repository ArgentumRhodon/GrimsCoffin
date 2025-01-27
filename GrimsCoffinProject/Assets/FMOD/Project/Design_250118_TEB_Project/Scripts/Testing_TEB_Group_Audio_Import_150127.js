studio.menu.addMenuItem({
    name: "Completed_TEB_Group Audio Import_V1.0_250127",
    //studio.window.browserCurrent().dump()
    isEnabled: function () {
        return true;
    },
    execute: function () {

        var dataTableComp = [
            "LM,MapDenial,WallBreak,001",
            "LM,MapDenial,SpikeFall,002",
            "LM,MapDenial,SpikeCollision,003",
            "LM,MapDenial,Waterdrop,004",
            "PL,Grim,Scyche_Swing,101",
            "PL,Grim,Scyche_Spin,102",
            "PL,Grim,Scyche_Slam,103",
            "PL,Grim,Scyche_DownAttack_Charging,104",
            "PL,Grim,Scyche_DownAttack_Release,105",
            "PL,Grim,Self_Hit,106",
            "PL,Grim,Self_Run,107",
            "PL,Grim,Self_Jump,108",
            "PL,Grim,Self_Dash,109",
            "PL,Grim,Self_WallSlide,110",
            "PL,Grim,Self_Land,111",
            "PL,Grim,Self_WallLeap,112",
            "ENM,Skeleton,Injured,201",
            "ENM,Ghost,Injured,202",
            "ENM,Skeleton,Die,203",
            "ENM,Ghost,Die,204"
        ];


        /*var sampleDataTableComp = [
            "LM,Level,Door_On,001",
            "LM,Level,Door_NA_01,002",
            "LM,Level,Door_Off,003",
            "LM,Level,Door_Wood,004",
            "LM,Level,Elevator_Open,005",
            "LM,WD,TV,006",
            "LM,WD,LPTP,007",
            "LM,WD,Window,008",
            "LM,WD,Glass,009",
            "LM,WD,PorceLain,010",
            "LM,WD,Wind,011",
            "LM,WD,Lightning,012",
            "LM,WD,Thunder,013",
            "LM,WD,Rain_Small,014",
            "LM,WD,Rain_Big,015",
            "PL,Hard,Footsteps,101",
            "PL,Soft,footsteps,102",
            "PL,Wood,footsteps,103",
            "PL,misc,PlayerLoot,104",
            "PL,misc,PlayerInteract,105",
            "PL,misc,PlayerInjured,106",
            "PL,misc,PlayerHpPick,107",
            "PL,PlayerShootHG,Normal,108",
            "PL,PlayerShootHG,LowAmmo,109",
            "PL,PlayerShootSTG,Normal,110",
            "PL,PlayerShootSTG,LowAmmo,111",
            "PL,misc,NoAmmo,112",
            "PL,misc,Reload,113",
            "PL,misc,AmmoPick,114",
            "PL,misc,ItemPick,115",
            "PL,Shot,Perfect,116",
            "PL,Shot,Good,117",
            "PL,Shot,Miss,118",
            "ENM,misc,AIAlert,201",
            "ENM,misc,AIInjured,202",
            "ENM,misc,AIEliminated,203",
            "ENM,misc,AIDrop,204",
            "BOSS,Boss,Fire_Single,301",
            "BOSS,Boss,Fire_Fullauto,302",
            "BOSS,Boss,Fire_CDstart_Kick_Steam,303",
            "BOSS,Boss,Fire_CDend,304",
            "BOSS,Boss,Dmaged_Shot,305",
            "BOSS,Boss,Dmaged_Explosion_S,306",
            "BOSS,Boss,Dmaged_Explosion_M,307",
            "BOSS,Boss,Dmaged_Explosion_L,308",
            "BOSS,Boss,Move_Slow,309",
            "BOSS,Boss,Move_Fast,310",
            "BOSS,Boss,Move_Gun,311",
            "BOSS,Boss,Move_Wire,312",
            "BOSS,Boss,Fire_Electrify,313",
            "UI,misc,Heartbeat,401",
            "UI,misc,Fatality,402",
            "UI,misc,KeyItemPick,403",
            "UI,Side,Compelte_Updated,404",
            "UI,Side,Failure_Updated,405",
            "UI,Menu,Pause_Updated,406",
            "UI,Menu,Resume_Updated,407",
            "UI,Menu,Select,408",
            "UI,Menu,Enter,409"
        ];
        */


        function soundLoaderMacro(typeName, categoryName, eventName, serialNumber, currentLength, receivedEvent) {

            //Dataset for all the collection
            {
                //console.log(receivedEvent.name);
                var newCategoryName = "";
                if (categoryName != "misc") {
                    //console.log("This is not misc name so it will be normalized");
                    newCategoryName = categoryName + "_";
                }
                var spinState = true;
                var index = 0
                var resultLength = 0;
                var typeFolderAddress = "event:/" + typeName;
                var typeFolder = studio.project.lookup(typeFolderAddress);
                var categoryFolderAddress = "event:/" + typeName + "/" + categoryName;
                var categoryFolder = studio.project.lookup(categoryFolderAddress);
                var eventAddress = "event:/" + typeName + "/" + categoryName + "/" + serialNumber + "_" + eventName;
                var namedEvent = studio.project.lookup(eventAddress);
                var assetFolderPath =
                    "asset:/" + "FX_" + serialNumber + "_" + typeName + "_" + newCategoryName + eventName;
                var assetFilePath = assetFolderPath +
                    "/" + "FX_" + serialNumber + "_" + typeName + "_" + newCategoryName + eventName;
                var specificAssetPath = assetFilePath + "_" + numberProcessor(index) + ".wav";
                function numberProcessor(inputNumber) {
                    var realNumber = parseInt(inputNumber);
                    var finalNumber = 0;
                    if (realNumber < 8) {
                        finalNumber = "0" + (realNumber + 1)
                    }
                    if (realNumber > 8 && realNumber < 98) {
                        (realNumber + 1)
                    }
                    return finalNumber;
                }
                function pathFinder(inputFilePath, inputIndex) {
                    var outputPath = inputFilePath + "_" + numberProcessor(inputIndex) + ".wav";
                    return outputPath;
                }
            }
            var testPath = studio.project.lookup(pathFinder(assetFilePath, 0));

            if (typeof receivedEvent.groupTracks[0].modules[0] == "undefined" && typeof testPath != "undefined") {
                var multiSound = receivedEvent.groupTracks[0].addSound(receivedEvent.timeline, 'MultiSound', 0, testPath.length);
                while (spinState) {
                    //console.log(pathFinder(assetFilePath, index));
                    var loggingAsset = studio.project.lookup(pathFinder(assetFilePath, index));
                    var currentLength = 0;
                    var assetIdentity = typeof loggingAsset;

                    if (assetIdentity != "undefined") {
                        //currentLength = loggingAsset.length;
                        var subsound = studio.project.create('SingleSound');
                        subsound.audioFile = loggingAsset;
                        multiSound.relationships.sounds.add(subsound);
                        //resultLength += currentLength;
                        index++;
                    }
                    else {
                        spinState = false;
                    }
                }
            }

            if (index > 0) {
                console.log("|");
                console.log("Success: ");
                console.log("Sound Creation for " + receivedEvent.name + " is completed!")
            }
            else if (typeof receivedEvent.groupTracks[0].modules[0] != "undefined") {
                console.log("|");
                console.log("Caution: ");
                console.log("Sound Creation for " + receivedEvent.name + " has already been done!")
            }
            else {
                console.log("|");
                console.log("Failure: ");
                console.log("Sound Creation for " + receivedEvent.name + " is not successful! The naming structure does not follow the convention")     
            }
            
        }
        function folderCreator(typeName, categoryName, eventName, serialNumber) {
            // Name Checker Begins

            var typeFolderAddress = "event:/" + typeName;
            var typeFolder = studio.project.lookup(typeFolderAddress);

            var categoryFolderAddress = "event:/" + typeName + "/" + categoryName;
            var categoryFolder = studio.project.lookup(categoryFolderAddress);
            //var secondCheckEventType = categoryFolder.entity;
            var eventAddress = "event:/" + typeName + "/" + categoryName + "/" + serialNumber + "_" + eventName;
            var namedEvent = studio.project.lookup(eventAddress);
            //var thirdCheckEventType = namedEvent.entity;
            var processCompletion = true;
            var bankSFX = studio.project.lookup("bank:/Master");
            var busSFX = studio.project.lookup("bus:/SFX");


            if (typeof typeFolder == "undefined") {
                //console.log("Top level folder yet empty, creating folder " + typeFolderAddress);
                var folder = studio.project.create('EventFolder');
                folder.name = typeName;

            }
            if (typeof categoryFolder == "undefined") {

                //console.log("Secondary level folder yet empty, creating folder " + typeFolderAddress);
                var folder = studio.project.create('EventFolder');
                folder.name = categoryName;
                folder.folder = studio.project.lookup(typeFolderAddress);

            }
            if (typeof namedEvent != "object") {
                //console.log("Can't find any event inside the events folder, will be creating one:)");
                var event = studio.project.create('Event');
                event.name = serialNumber + "_" + eventName;
                event.folder = studio.project.lookup(categoryFolderAddress);
                event.relationships.banks.add(bankSFX);
                event.mixerInput.output = busSFX;
                event.addGroupTrack("1");
                //console.log("Event creation for " + event.name + " is successful!");
                return event;
                
                
            }

            if (typeof namedEvent == "object") {
                //console.log(namedEvent.name + "has been created, will now try to import audio to it nonetheless")
                return namedEvent;
            }
      
            else {
                //console.log("The event itself has been created already and is called: " + serialNumber + "_" + eventName + " in the main folder " + typeName + " and sub-folder " + categoryName);
            }

        }//This whole thing basically checks if somethting exists or not.
        function creationLoop(data) {
            data.forEach(function (line, index) {
                //console.log("Item " + (index + 1) + ": " + line);
                var parts = line.split(',');
                //console.log("Type: " + parts[0] + ", Category: " + parts[1] + ", Name: " + parts[2] + ", Serial Number: " + parts[3]);
                var type = parts[0];
                var category = parts[1];
                var name = parts[2];
                var sn = parts[3];
                var createdEvent = folderCreator(type, category, name, sn);
                // Create a new event using studio.project.create with explicit event type
                if (typeof createdEvent != "undefined") {
                    var returnName = soundLoaderMacro(type, category, name, sn, 0, createdEvent);

                }
                else {
                    //console.log("Type: " + parts[0] + ", Category: " + parts[1] + ", Name: " + parts[2] + ", Serial Number: " + parts[3] + " has either been created or undefined");
                }
                
           
            });
        }

        creationLoop(dataTableComp);
    }
});
