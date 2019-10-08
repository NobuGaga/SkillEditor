KeyFrameData = Class("KeyFrameData", ConfigBase)
KeyFrameData.config = {
    ["hero_nwang"] = {
        ["attack_1"] = {
            [1] = {
                name = "start",
                time = 0,
                priority = 99,
                data = {},
            },
            [2] = {
                name = "play_effect",
                time = 0,
                priority = 1,
                data = {
                    [1] = {
                        type = 1,
                        id = 10,
                    },
                    [2] = {
                        type = 3,
                        id = 100,
                    },
                },
            },
            [3] = {
                name = "cache_begin",
                time = 0.2,
                priority = 99,
                data = {},
            },
            [4] = {
                name = "hit",
                time = 0.133,
                priority = 1,
                data = {
                    [4] = {
                        [1] = {
                            x = -1.4,
                            y = 0,
                            z = 0,
                            width = 3.5,
                            height = 1.7,
                            depth = 2
                        },
                    },
                },
            },
        }
    }
}