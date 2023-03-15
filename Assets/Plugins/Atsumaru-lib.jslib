
mergeInto(LibraryManager.library, {
    // check Atumaru available
    atsumaru_isValid: function () {
        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            return true;
        }
        else {
            return false;
        }
    },

    // get master volume
    atsumaru_getVolume: function () {
        let volume = undefined;
        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            volume = atsumaru.volume.getCurrentValue();
        }
        return volume;
    },

    // set master volume change callback
    atsumaru_onChangeVolume: function (gameObject, methodName) {
        // console.log("atsumaru_onChangeVolume() in gameObject:" + gameObject);
        // console.log("atsumaru_onChangeVolume() in methodName:" + methodName);

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);

        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            atsumaru.volume.changed.subscribe((volume) => {

                var param = {
                    volume: volume,
                };
                var json = JSON.stringify(param);
                myGameInstance.SendMessage(strGameObject, strMethodName, json);

                // console.log('atsumaru_onChangeVolume volume:' + volume);
                // console.log('atsumaru_onChangeVolume json:' + json);
                // console.log('atsumaru_onChangeVolume GameObject:' + strGameObject);
                // console.log('atsumaru_onChangeVolume MethodName:' + strMethodName);
            });
        }
    },

    // set screenshot handler to atsumaru api.
    atsumaru_onScreenCapture: function (gameObject, methodName) {
        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        // console.log('atsumaru_onScreenCapture GameObject:' + strGameObject);
        // console.log('atsumaru_onScreenCapture MethodName:' + strMethodName);

        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            if (atsumaru.experimental == null) {
                return;
            }
            if (atsumaru.experimental.screenshot == null) {
                return;
            }
            if (atsumaru.experimental.screenshot.setScreenshotHandler == null) {
                return;
            }

            // console.log("atsumaru_onScreenCapture() ScreenshotHandler available.");

            atsumaru.experimental.screenshot.setScreenshotHandler(async () => {
                // console.log("atsumaru_onScreenCapture() ScreenshotHandler called");

                // screen captuer callback to unity
                myGameInstance.SendMessage(strGameObject, strMethodName);
                // console.log("atsumaru_onScreenCapture() sleep start.");
                // sleep
                await myGameInstance.Module.AtsumaruAPI._sleep(100);
                // console.log("atsumaru_onScreenCapture() sleep end.");

                var capture = document.getElementById("capture");
                return capture.src;
            });
        }
    },

    // set captuer image
    atsumaru_setScreenCapture: function (img, size) {
        // console.log("atsumaru_setScreenCapture() called.");

        var binary = '';
        for (var i = 0; i < size; i++)
            binary += String.fromCharCode(HEAPU8[img + i]);
        var dataUrl = 'data:image/png;base64,' + btoa(binary);

        var capture = document.getElementById("capture");
        capture.src = dataUrl;
    },

    atsumaru_loadServerData: function (gameObject, methodName) {
        const FAIL = 1;
        const SUCCESS = 0;

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        // console.log('atsumaru_onScreenCapture GameObject:' + strGameObject);
        // console.log('atsumaru_onScreenCapture MethodName:' + strMethodName);

        // get data from server
        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            // console.log("Atsumaru atsumaru.storage.getItems() start.");
            atsumaru.storage.getItems()
                .then(items => {
                    console.log("Atsumaru atsumaru.storage.getItems() success.");

                    const json = myGameInstance.Module.AtsumaruAPI._storageItemToJson(SUCCESS, items);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                })
                .catch((error) => {
                    console.log("Atsumaru atsumaru.storage.getItems() fail.");
                    console.error(error.message);

                    const json = myGameInstance.Module.AtsumaruAPI._storageItemToJson(FAIL, []);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                });
        }
        else {
            // console.log("Atsumaru not in work.");
            const json = myGameInstance.Module.AtsumaruAPI._storageItemToJson(FAIL, []);
            myGameInstance.SendMessage(strGameObject, strMethodName, json);
        }
    },

    atsumaru_saveServerData: function (gameObject, methodName, dataJson) {
        const FAIL = 1;
        const SUCCESS = 0;

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        var strDataJson = UTF8ToString(dataJson);
        // console.log('atsumaru_saveServerData GameObject:' + strGameObject);
        // console.log('atsumaru_saveServerData MethodName:' + strMethodName);
        // console.log('atsumaru_saveServerData DataJson:' + strDataJson);

        var serverItems = JSON.parse(strDataJson);
        // for (let i = 0; i < serverItems.data.length; i++) {
        //     console.log('atsumaru_saveServerData data[' + i + '].key:' + serverItems.data[i].key);
        //     console.log('atsumaru_saveServerData data[' + i + '].value:' + serverItems.data[i].value);
        // }

        // set data to server
        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            // console.log("Atsumaru atsumaru.storage.setItems() start.");
            atsumaru.storage.setItems(serverItems.data)
                .then((value) => {
                    console.log("Atsumaru atsumaru.storage.setItems() success.");

                    const json = myGameInstance.Module.AtsumaruAPI._statToJson(SUCCESS);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                })
                .catch((error) => {
                    console.log("Atsumaru atsumaru.storage.setItems() fail.");
                    console.error(error.message);

                    const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                });
        }
        else {
            // console.log("Atsumaru not in work.");
            const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
            myGameInstance.SendMessage(strGameObject, strMethodName, json);
        };
    },

    atsumaru_deleteServerData: function (gameObject, methodName, dataJson) {
        const FAIL = 1;
        const SUCCESS = 0;

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        var strDataJson = UTF8ToString(dataJson);
        // console.log('atsumaru_deleteServerData GameObject:' + strGameObject);
        // console.log('atsumaru_deleteServerData MethodName:' + strMethodName);
        // console.log('atsumaru_deleteServerData DataJson:' + strDataJson);

        var storageItem = JSON.parse(strDataJson);
        // console.log('atsumaru_deleteServerData key:' + storageItem.key);

        // delete data from server
        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            // console.log("Atsumaru atsumaru.storage.removeItem() start.");
            atsumaru.storage.removeItem(storageItem.key)
                .then((value) => {
                    console.log("Atsumaru atsumaru.storage.removeItem() success.");

                    const json = myGameInstance.Module.AtsumaruAPI._statToJson(SUCCESS);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                })
                .catch((error) => {
                    console.log("Atsumaru atsumaru.storage.removeItem() fail.");
                    console.error(error.message);

                    const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                });
        }
        else {
            // console.log("Atsumaru not in work.");
            const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
            myGameInstance.SendMessage(strGameObject, strMethodName, json);
        }
    },

    atsumaru_saveScoreBoard: function (gameObject, methodName, boardId, score) {
        const FAIL = 1;
        const SUCCESS = 0;

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        // console.log('atsumaru_saveScoreBoard GameObject:' + strGameObject);
        // console.log('atsumaru_saveScoreBoard MethodName:' + strMethodName);
        // console.log('atsumaru_saveScoreBoard BoardId:' + boardId);
        // console.log('atsumaru_saveScoreBoard Score:' + score);

        // save sore to server
        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            if (atsumaru.experimental == null) {
                return;
            }
            if (atsumaru.experimental.scoreboards == null) {
                return;
            }
            if (atsumaru.experimental.scoreboards.setRecord == null) {
                return;
            }

            // console.log("Atsumaru atsumaru.experimental.scoreboards.setRecord() start.");
            atsumaru.experimental.scoreboards.setRecord(boardId, score)
                .then((value) => {
                    console.log("Atsumaru atsumaru.experimental.scoreboards.setRecord() success.");

                    const json = myGameInstance.Module.AtsumaruAPI._statToJson(SUCCESS);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                })
                .catch((error) => {
                    console.log("Atsumaru atsumaru.experimental.scoreboards.setRecord() fail.");
                    console.error(error.message);

                    const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                });
        }
        else {
            // console.log("Atsumaru not in work.");
            const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
            myGameInstance.SendMessage(strGameObject, strMethodName, json);
        }
    },

    atsumaru_displayScoreBoard: function (gameObject, methodName, boardId) {
        const FAIL = 1;
        const SUCCESS = 0;

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        // console.log('atsumaru_displayScoreBoard GameObject:' + strGameObject);
        // console.log('atsumaru_displayScoreBoard MethodName:' + strMethodName);
        // console.log('atsumaru_displayScoreBoard BoardId:' + boardId);

        // display sore
        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            if (atsumaru.experimental == null) {
                return;
            }
            if (atsumaru.experimental.scoreboards == null) {
                return;
            }
            if (atsumaru.experimental.scoreboards.setRecord == null) {
                return;
            }

            // console.log("Atsumaru atsumaru.experimental.scoreboards.display() start.");
            atsumaru.experimental.scoreboards.display(boardId)
                .then((value) => {
                    console.log("Atsumaru atsumaru.experimental.scoreboards.display() success.");

                    const json = myGameInstance.Module.AtsumaruAPI._statToJson(SUCCESS);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                })
                .catch((error) => {
                    console.log("Atsumaru atsumaru.experimental.scoreboards.display() fail.");
                    console.error(error.message);

                    const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
                    myGameInstance.SendMessage(strGameObject, strMethodName, json);
                });
        }
        else {
            // console.log("Atsumaru not in work.");
            const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
            myGameInstance.SendMessage(strGameObject, strMethodName, json);
        }
    },

});
