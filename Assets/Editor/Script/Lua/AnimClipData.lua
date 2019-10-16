--*-coding:utf-8-*
--Import Table Start
-- 表格定义维度如下：
-- [CharacterDefine.TYPE][StatesConfig][AnimName]
AnimClipData = AnimClipData or {}
AnimClipData.data = {
    ["hero_nvwang"] = {
        ["EntityStateDefine.Atk"] = {
            ["attack_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    ["Hit"] = {
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
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
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
                    ["CacheBegin"] = {
                        time = 0.2,
                        priority = 99,
                        data = {},
                    },
                    ["SectionOver"] = {
                        time = 0.27,
                        priority = 1,
                        data = {},
                    },
                    ["End"] = {
                        time = 0.867,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["attack_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    ["Hit"] = {
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
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
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
                    ["CacheBegin"] = {
                        time = 0.2,
                        priority = 99,
                        data = {},
                    },
                    ["SectionOver"] = {
                        time = 0.27,
                        priority = 1,
                        data = {},
                    },
                    ["End"] = {
                        time = 0.967,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["attack_3"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    ["Hit"] = {
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
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
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
                    ["CacheBegin"] = {
                        time = 0.4,
                        priority = 99,
                        data = {},
                    },
                    ["SectionOver"] = {
                        time = 0.8,
                        priority = 1,
                        data = {},
                    },
                    ["End"] = {
                        time = 1.333,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["attack_4_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    ["Hit"] = {
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
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
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
                    ["End"] = {
                        time = 0.467,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["attack_4_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    ["Hit"] = {
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
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
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
                    ["CacheBegin"] = {
                        time = 0.368,
                        priority = 99,
                        data = {},
                    },
                    ["SectionOver"] = {
                        time = 0.368,
                        priority = 1,
                        data = {},
                    },
                    ["End"] = {
                        time = 0.833,
                        priority = 1,
                        data = {},
                    },
                },
            },
        },
        ["EntityStateDefine.UseSkill"] = {
            ["skill_01_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 30,
                            },
                        },
                    },
                    ["End"] = {
                        time = 1.467,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["skill_01_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 31,
                            },
                        },
                    },
                    ["End"] = {
                        time = 1.133,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["skill_02_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 32,
                            },
                        },
                    },
                    ["End"] = {
                        time = 0.233,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["skill_02_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 33,
                            },
                        },
                    },
                    ["End"] = {
                        time = 1.267,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["skill_03_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 34,
                            },
                        },
                    },
                    ["End"] = {
                        time = 1.1,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["skill_03_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 35,
                            },
                        },
                    },
                    ["End"] = {
                        time = 2.067,
                        priority = 1,
                        data = {},
                    },
                },
            },
        },
        ["EntityStateDefine.Hit"] = {
            ["hit_1"] = {
                iPoolType = GameConstant.POOL_ANIM_HIT,
            },
            ["hit_2"] = {
                iPoolType = GameConstant.POOL_ANIM_HIT,
            },
        },
        ["EntityStateDefine.Dead"] = {
            ["die"] = {
                iPoolType = GameConstant.POOL_ANIM_DEFAULT,
            },
        },
    },
    ["monster_xiaoguai"] = {
        ["EntityStateDefine.Hit"] = {
            ["hit_1"] = {
                iPoolType = GameConstant.POOL_ANIM_HIT,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {},
                    },
                    ["End"] = {
                        time = 0.467,
                        priority = 1,
                        data = {},
                    },
                },
            },
            ["hit_2"] = {
                iPoolType = GameConstant.POOL_ANIM_HIT,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                        data = {},
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {},
                    },
                    ["End"] = {
                        time = 0.467,
                        priority = 1,
                        data = {},
                    },
                },
            },
        },
        ["EntityStateDefine.Dead"] = {
            ["die"] = {
                iPoolType = GameConstant.POOL_ANIM_DEFAULT,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 1,
                        data = {},
                    },
                    ["End"] = {
                        time = 2,
                        priority = 1,
                        data = {},
                    },
                },
            },
        },
    },
}