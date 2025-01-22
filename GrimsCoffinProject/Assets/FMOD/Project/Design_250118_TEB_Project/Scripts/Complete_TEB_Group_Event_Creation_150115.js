studio.menu.addMenuItem({
    name: "Complete_TEB_Group Event Creation_V1.0",
    //studio.window.browserCurrent().dump()
    isEnabled: function () {
        return true;
    },
    execute: function () {

        var dataTableComp = [
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

        /*var dataTableZelda = [
            "Hyrule, wind, hyrule_wind_strong_wind, 001",
            "Hyrule, wind, hyrule_wind_weak_wind, 002",
            "Hyrule, wind, hyrule_wind_gentle_breeze, 003",
            "Hyrule, wind, hyrule_wind_storm_gust, 004",
            "Hyrule, wind, hyrule_wind_whistling_wind, 005",
            "Hyrule, forest, hyrule_forest_rustling_leaves, 006",
            "Hyrule, forest, hyrule_forest_bird_chirping, 007",
            "Hyrule, forest, hyrule_forest_insects, 008",
            "Hyrule, forest, hyrule_forest_wind_through_trees, 009",
            "Hyrule, village, hyrule_village_children_playing, 010",
            "Hyrule, village, hyrule_village_marketplace_noise, 011",
            "Hyrule, village, hyrule_village_footsteps_on_gravel, 012",
            "Hyrule, village, hyrule_village_distant_chatter, 013",
            "Hyrule, village, hyrule_village_animal_sounds, 014",
            "Hyrule, cave, hyrule_cave_dripping_water, 015",
            "Hyrule, cave, hyrule_cave_footsteps_on_stone, 016",
            "Hyrule, cave, hyrule_cave_echo, 017",
            "Hyrule, cave, hyrule_cave_mysterious_hum, 018",
            "Hyrule, cave, hyrule_cave_wind_in_cavern, 019",
            "Hyrule, battle, hyrule_battle_sword_clang, 020",
            "Hyrule, battle, hyrule_battle_arrow_shot, 021",
            "Hyrule, battle, hyrule_battle_explosion, 022",
            "Hyrule, battle, hyrule_battle_enemy_shout, 023",
            "Hyrule, battle, hyrule_battle_magic_cast, 024",

            // Additional variations
            "Hyrule, wind, hyrule_wind_strong_wind_var1, 025",
            "Hyrule, wind, hyrule_wind_weak_wind_var2, 026",
            "Hyrule, wind, hyrule_wind_gentle_breeze_var3, 027",
            "Hyrule, wind, hyrule_wind_storm_gust_var4, 028",
            "Hyrule, wind, hyrule_wind_whistling_wind_var5, 029",
            "Hyrule, forest, hyrule_forest_rustling_leaves_var6, 030",
            "Hyrule, forest, hyrule_forest_bird_chirping_var7, 031",
            "Hyrule, forest, hyrule_forest_insects_var8, 032",
            "Hyrule, forest, hyrule_forest_wind_through_trees_var9, 033",
            "Hyrule, village, hyrule_village_children_playing_var10, 034",
            "Hyrule, village, hyrule_village_marketplace_noise_var11, 035",
            "Hyrule, village, hyrule_village_footsteps_on_gravel_var12, 036",
            "Hyrule, village, hyrule_village_distant_chatter_var13, 037",
            "Hyrule, village, hyrule_village_animal_sounds_var14, 038",
            "Hyrule, cave, hyrule_cave_dripping_water_var15, 039",
            "Hyrule, cave, hyrule_cave_footsteps_on_stone_var16, 040",
            "Hyrule, cave, hyrule_cave_echo_var17, 041",
            "Hyrule, cave, hyrule_cave_mysterious_hum_var18, 042",
            "Hyrule, cave, hyrule_cave_wind_in_cavern_var19, 043",
            "Hyrule, battle, hyrule_battle_sword_clang_var20, 044",
            "Hyrule, battle, hyrule_battle_arrow_shot_var21, 045",
            "Hyrule, battle, hyrule_battle_explosion_var22, 046",
            "Hyrule, battle, hyrule_battle_enemy_shout_var23, 047",
            "Hyrule, battle, hyrule_battle_magic_cast_var24, 048",

            "Hyrule, wind, hyrule_wind_strong_wind_var25, 049",
            "Hyrule, wind, hyrule_wind_weak_wind_var26, 050",
            "Hyrule, wind, hyrule_wind_gentle_breeze_var27, 051",
            "Hyrule, wind, hyrule_wind_storm_gust_var28, 052",
            "Hyrule, wind, hyrule_wind_whistling_wind_var29, 053",
            "Hyrule, forest, hyrule_forest_rustling_leaves_var30, 054",
            "Hyrule, forest, hyrule_forest_bird_chirping_var31, 055",
            "Hyrule, forest, hyrule_forest_insects_var32, 056",
            "Hyrule, forest, hyrule_forest_wind_through_trees_var33, 057",
            "Hyrule, village, hyrule_village_children_playing_var34, 058",
            "Hyrule, village, hyrule_village_marketplace_noise_var35, 059",
            "Hyrule, village, hyrule_village_footsteps_on_gravel_var36, 060",
            "Hyrule, village, hyrule_village_distant_chatter_var37, 061",
            "Hyrule, village, hyrule_village_animal_sounds_var38, 062",
            "Hyrule, cave, hyrule_cave_dripping_water_var39, 063",
            "Hyrule, cave, hyrule_cave_footsteps_on_stone_var40, 064",
            "Hyrule, cave, hyrule_cave_echo_var41, 065",
            "Hyrule, cave, hyrule_cave_mysterious_hum_var42, 066",
            "Hyrule, cave, hyrule_cave_wind_in_cavern_var43, 067",
            "Hyrule, battle, hyrule_battle_sword_clang_var44, 068",
            "Hyrule, battle, hyrule_battle_arrow_shot_var45, 069",
            "Hyrule, battle, hyrule_battle_explosion_var46, 070",
            "Hyrule, battle, hyrule_battle_enemy_shout_var47, 071",
            "Hyrule, battle, hyrule_battle_magic_cast_var48, 072",

            "Hyrule, wind, hyrule_wind_strong_wind_var49, 073",
            "Hyrule, wind, hyrule_wind_weak_wind_var50, 074",
            "Hyrule, wind, hyrule_wind_gentle_breeze_var51, 075",
            "Hyrule, wind, hyrule_wind_storm_gust_var52, 076",
            "Hyrule, wind, hyrule_wind_whistling_wind_var53, 077",
            "Hyrule, forest, hyrule_forest_rustling_leaves_var54, 078",
            "Hyrule, forest, hyrule_forest_bird_chirping_var55, 079",
            "Hyrule, forest, hyrule_forest_insects_var56, 080",
            "Hyrule, forest, hyrule_forest_wind_through_trees_var57, 081",
            "Hyrule, village, hyrule_village_children_playing_var58, 082",
            "Hyrule, village, hyrule_village_marketplace_noise_var59, 083",
            "Hyrule, village, hyrule_village_footsteps_on_gravel_var60, 084",
            "Hyrule, village, hyrule_village_distant_chatter_var61, 085",
            "Hyrule, village, hyrule_village_animal_sounds_var62, 086",
            "Hyrule, cave, hyrule_cave_dripping_water_var63, 087",
            "Hyrule, cave, hyrule_cave_footsteps_on_stone_var64, 088",
            "Hyrule, cave, hyrule_cave_echo_var65, 089",
            "Hyrule, cave, hyrule_cave_mysterious_hum_var66, 090",
            "Hyrule, cave, hyrule_cave_wind_in_cavern_var67, 091",
            "Hyrule, battle, hyrule_battle_sword_clang_var68, 092",
            "Hyrule, battle, hyrule_battle_arrow_shot_var69, 093",
            "Hyrule, battle, hyrule_battle_explosion_var70, 094",
            "Hyrule, battle, hyrule_battle_enemy_shout_var71, 095",
            "Hyrule, battle, hyrule_battle_magic_cast_var72, 096",
            "Hyrule, wind, hyrule_wind_strong_wind_var73, 097",
            "Hyrule, wind, hyrule_wind_weak_wind_var74, 098",
            "Hyrule, wind, hyrule_wind_gentle_breeze_var75, 099",
            "Hyrule, wind, hyrule_wind_storm_gust_var76, 100",

            // Player effects
            "Player, sword, player_sword_attack, 400",
            "Player, bow, player_bow_attack, 401",
            "Player, shield, player_shield_block, 402",
            "Player, jump, player_jump_attack, 403",
            "Player, magic, player_magic_cast, 404",
            "Player, sword, player_sword_attack, 405",
            "Player, bow, player_bow_attack, 406",
            "Player, shield, player_shield_block, 407",
            "Player, jump, player_jump_attack, 408",
            "Player, magic, player_magic_cast, 409",
            "Player, sword, player_sword_attack, 410",
            "Player, bow, player_bow_attack, 411",
            "Player, shield, player_shield_block, 412",
            "Player, jump, player_jump_attack, 413",
            "Player, magic, player_magic_cast, 414",
            "Player, sword, player_sword_attack, 415",
            "Player, bow, player_bow_attack, 416",
            "Player, shield, player_shield_block, 417",
            "Player, jump, player_jump_attack, 418",
            "Player, magic, player_magic_cast, 419",
            "Player, sword, player_sword_attack, 420",
            "Player, bow, player_bow_attack, 421",
            "Player, shield, player_shield_block, 422",
            "Player, jump, player_jump_attack, 423",
            "Player, magic, player_magic_cast, 424",
            "Player, sword, player_sword_attack, 425",
            "Player, bow, player_bow_attack, 426",
            "Player, shield, player_shield_block, 427",
            "Player, jump, player_jump_attack, 428",
            "Player, magic, player_magic_cast, 429",

            // Boss effects
            "Boss, attack, boss_attack_magic, 500",
            "Boss, attack, boss_attack_strike, 501",
            "Boss, roar, boss_roar, 502",
            "Boss, teleport, boss_teleport, 503",
            "Boss, laugh, boss_laugh, 504",
            "Boss, attack, boss_attack_magic, 505",
            "Boss, attack, boss_attack_strike, 506",
            "Boss, roar, boss_roar, 507",
            "Boss, teleport, boss_teleport, 508",
            "Boss, laugh, boss_laugh, 509",
            "Boss, attack, boss_attack_magic, 510",
            "Boss, attack, boss_attack_strike, 511",
            "Boss, roar, boss_roar, 512",
            "Boss, teleport, boss_teleport, 513",
            "Boss, laugh, boss_laugh, 514",
            "Boss, attack, boss_attack_magic, 515",
            "Boss, attack, boss_attack_strike, 516",
            "Boss, roar, boss_roar, 517",
            "Boss, teleport, boss_teleport, 518",
            "Boss, laugh, boss_laugh, 519",
            "Boss, attack, boss_attack_magic, 520",
            "Boss, attack, boss_attack_strike, 521",
            "Boss, roar, boss_roar, 522",
            "Boss, teleport, boss_teleport, 523",
            "Boss, laugh, boss_laugh, 524",
            "Boss, attack, boss_attack_magic, 525",
            "Boss, attack, boss_attack_strike, 526",
            "Boss, roar, boss_roar, 527",
            "Boss, teleport, boss_teleport, 528",
            "Boss, laugh, boss_laugh, 529"
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
                        finalNumber = "00" + (realNumber + 1)
                    }
                    if (realNumber > 8 && realNumber < 98) {
                        finalNumber = "0" + (realNumber + 1)
                    }
                    if (realNumber > 98) {
                        finalNumber = (realNumber + 1)
                    }
                    return finalNumber;
                }
                function pathFinder(inputFilePath, inputIndex) {
                    var outputPath = inputFilePath + "_" + numberProcessor(inputIndex) + ".wav";
                    return outputPath;
                }
            }

            //console.log(typeof (studio.project.lookup(assetFilePath + "_" + serialNumber + ".wav")));

            //console.log(pathFinder(assetFilePath, index));
            //console.log(pathFinder(assetFilePath, index));
            while (spinState) {
                //console.log(pathFinder(assetFilePath, index));
                var loggingAsset = studio.project.lookup(pathFinder(assetFilePath, index));
                var currentLength = 0;
                if (typeof loggingAsset != "undefined") {
                    currentLength = loggingAsset.length;
                    var soundBrick = receivedEvent.groupTracks[0].addSound
                        (receivedEvent.timeline, 'SingleSound', resultLength, currentLength);
                    soundBrick.audioFile = loggingAsset;
                    resultLength += currentLength;
                    index++;
                }
                else {
                    spinState = false;
                }
                

            }
            if (index >= 0) {
                console.log("Sound Creation for " + receivedEvent.name+ " is successful!")
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
                console.log("Event creation for " + event.name + " is successful!");
                return event;
                
                
            }
            else {
                console.log("The event itself has been created already and is called: " + serialNumber + "_" + eventName +
                    " in the main folder " + typeName + " and sub-folder " + categoryName);
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
                    soundLoaderMacro(type, category, name, sn, 0, createdEvent);
                }
                
           
            });
        }

        creationLoop(dataTableComp);
    }
});
