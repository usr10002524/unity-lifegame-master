
mergeInto(LibraryManager.library, {

    loadLocalData: function (gameObject, methodName) {
        const FAIL = 1;
        const SUCCESS = 0;

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        // console.log('loadLocalData GameObject:' + strGameObject);
        // console.log('loadLocalData MethodName:' + strMethodName);

        myGameInstance.Module.LocalStorageAPI.getItems()
            .then(items => {
                // console.log("LocalStorageAPI.getItems() success.");

                const json = myGameInstance.Module.AtsumaruAPI._storageItemToJson(SUCCESS, items);
                myGameInstance.SendMessage(strGameObject, strMethodName, json);
            })
            .catch(error => {
                console.log("LocalStorageAPI.getItems() fail.");
                console.error(error);

                const json = myGameInstance.Module.AtsumaruAPI._storageItemToJson(FAIL, []);
                myGameInstance.SendMessage(strGameObject, strMethodName, json);
            });
    },

    saveLocalData: function (gameObject, methodName, dataJson) {
        const FAIL = 1;
        const SUCCESS = 0;

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        var strDataJson = UTF8ToString(dataJson);
        // console.log('saveLocalData GameObject:' + strGameObject);
        // console.log('saveLocalData MethodName:' + strMethodName);
        // console.log('saveLocalData DataJson:' + strDataJson);

        var serverItems = JSON.parse(strDataJson);
        // for (let i = 0; i < serverItems.data.length; i++) {
        //     console.log('saveLocalData data[' + i + '].key:' + serverItems.data[i].key);
        //     console.log('saveLocalData data[' + i + '].value:' + serverItems.data[i].value);
        // }

        myGameInstance.Module.LocalStorageAPI.setItems(serverItems.data)
            .then(() => {
                // console.log("LocalStorageAPI.setItems() success.");

                const json = myGameInstance.Module.AtsumaruAPI._statToJson(SUCCESS);
                myGameInstance.SendMessage(strGameObject, strMethodName, json);
            })
            .catch(error => {
                console.log("LocalStorageAPI.setItems() fail.");
                console.error(error);

                const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
                myGameInstance.SendMessage(strGameObject, strMethodName, json);
            });
    },

    deleteLocalData: function (gameObject, methodName, dataJson) {
        const FAIL = 1;
        const SUCCESS = 0;

        var strGameObject = UTF8ToString(gameObject);
        var strMethodName = UTF8ToString(methodName);
        var strDataJson = UTF8ToString(dataJson);
        // console.log('deleteLocalData GameObject:' + strGameObject);
        // console.log('deleteLocalData MethodName:' + strMethodName);
        // console.log('deleteLocalData DataJson:' + strDataJson);

        var storageItem = JSON.parse(strDataJson);
        // console.log('deleteLocalData key:' + storageItem.key);

        myGameInstance.Module.LocalStorageAPI.deleteItem(storageItem.key)
            .then(() => {
                // console.log("LocalStorageAPI.deleteItem() success.");

                const json = myGameInstance.Module.AtsumaruAPI._statToJson(SUCCESS);
                myGameInstance.SendMessage(strGameObject, strMethodName, json);
            })
            .catch(error => {
                console.log("LocalStorageAPI.deleteItem() fail.");
                console.error(error);

                const json = myGameInstance.Module.AtsumaruAPI._statToJson(FAIL);
                myGameInstance.SendMessage(strGameObject, strMethodName, json);
            });
    },
});
