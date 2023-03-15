
Module["AtsumaruAPI"] = {
    _withAtsumaru: function (fn) {
        // console.log("_withAtsumaru called. (in Module AtsumaruAPI)");
        const atsumaru = window.RPGAtsumaru;
        if (atsumaru) {
            fn(atsumaru);
        }
        else {
            // console.log("RPGAtsumaru not available.");
        }
    },

    _sleep: async function (ms) {
        return new Promise((resolve) => setTimeout(resolve, ms));
    },

    _storageItemToJson: function (stat, items) {
        let data = {
            stat: stat,
            data: []
        };

        if (items != null) {
            for (i = 0; i < items.length; i++) {
                data.data.push({ key: items[i].key, value: items[i].value });
            }
        }

        let json = JSON.stringify(data);
        return json;
    },

    _statToJson: function (stat) {
        let data = {
            stat: stat
        };

        let json = JSON.stringify(data);
        return json;
    },
}
