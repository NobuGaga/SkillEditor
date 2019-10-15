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
                            depth = 2,
                        },
                    },
                },
            },
            [5] = {
                name = "section_over",
                time = 0.27,
                priority = 1,
                data = {},
            },
            [6] = {
                name = "end",
                time = 0.867,
                priority = 1,
                data = {},
            },
        },
        ["attack_2"] = {
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
                        id = 11,
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
                time = 0.108,
                priority = 1,
                data = {
                    [4] = {
                        [1] = {
                            x = -0.5,
                            y = 0,
                            z = 0,
                            width = 3.5,
                            height = 1.7,
                            depth = 2,
                        },
                    },
                },
            },
            [5] = {
                name = "section_over",
                time = 0.27,
                priority = 1,
                data = {},
            },
            [6] = {
                name = "end",
                time = 0.967,
                priority = 1,
                data = {},
            },
        },
        ["attack_3"] = {
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
                        id = 12,
                    },
                    [2] = {
                        type = 2,
                        id = 20,
                    },
                    [3] = {
                        type = 3,
                        id = 100,
                    },
                },
            },
            [3] = {
                name = "cache_begin",
                time = 0.4,
                priority = 99,
                data = {},
            },
            [4] = {
                name = "nit",
                time = 0.39,
                priority = 1,
                data = {
                    [4] = {
                        [1] = {
                            x = -2,
                            y = 0,
                            z = 0,
                            width = 5.4,
                            height = 2,
                            depth = 2,
                        },
                    },
                },
            },
            [5] = {
                name = "section_over",
                time = 0.8,
                priority = 1,
                data = {},
            },
            [6] = {
                name = "end",
                time = 1.333,
                priority = 1,
                data = {},
            },
        },
        ["attack_4_1"] = {
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
                        id = 13,
                    },
                    [2] = {
                        type = 3,
                        id = 100,
                    },
                },
            },
            [3] = {
                name = "hit",
                time = 0.233,
                priority = 1,
                data = {
                    [4] = {
                        [1] = {
                            x = 0,
                            y = 0,
                            z = 0,
                            width = 5,
                            height = 1.7,
                            depth = 2,
                        },
                    },
                },
            },
            [4] = {
                name = "end",
                time = 0.467,
                priority = 1,
                data = {},
            },
        },
        ["attack_4_2"] = {
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
                        id = 14,
                    },
                    [2] = {
                        type = 3,
                        id = 100,
                    },
                },
            },
            [3] = {
                name = "hit",
                time = 0.233,
                priority = 1,
                data = {
                    [4] = {
                        [1] = {
                            x = 0,
                            y = 0,
                            z = 0,
                            width = 6,
                            height = 1.7,
                            depth = 2,
                        },
                    },
                },
            },
            [4] = {
                name = "cache_begin",
                time = 0.368,
                priority = 99,
                data = {},
            },
            [5] = {
                name = "section_over",
                time = 0.368,
                priority = 1,
                data = {},
            },
            [6] = {
                name = "end",
                time = 0.833,
                priority = 1,
                data = {},
            },
        },
    },
    ["monster_xiaoguai"] = {
        ["hit_1"] = {
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
                data = {},
            },
            [3] = {
                name = "end",
                time = 0.467,
                priority = 1,
                data = {},
            },
        },
        ["hit_2"] = {
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
                data = {},
            },
            [3] = {
                name = "end",
                time = 0.467,
                priority = 1,
                data = {},
            },
        },
        ["die"] = {
            [1] = {
                name = "start",
                time = 0,
                priority = 1,
                data = {},
            },
            [2] = {
                name = "end",
                time = 2,
                priority = 1,
                data = {},
            },
        }
    }
}