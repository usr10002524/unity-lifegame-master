
Module["LocalStorageAPI"] = {

    getItems: async function () {
        try {
            let result = [];

            const keyCount = localStorage.length;
            for (let i = 0; i < keyCount; i++) {
                const key = localStorage.key(i);
                if (key == null) {
                    continue;
                }
                const value = localStorage.getItem(key);
                if (value == null) {
                    continue;
                }
                result.push({ key: key, value: value });
            }
            return result;
        }
        catch (e) {
            throw e;
        }
    },

    setItems: async function (data) {
        try {
            data.forEach(element => {
                localStorage.setItem(element.key, element.value);
            });

            return;
        }
        catch (e) {
            throw e;
        }
    },

    deleteItem: async function (key) {
        try {
            localStorage.removeItem(key);
            return;
        }
        catch (e) {
            throw e;
        }
    },
};