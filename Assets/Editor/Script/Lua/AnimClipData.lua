AnimClipData = AnimClipData or {}
AnimClipData.data = {
    ["hero_nvwang"] = {
        ["EntityStateDefine.Atk"] = {
            ["attack_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
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
                    },
                    ["SectionOver"] = {
                        time = 0.27,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 0.867,
                        priority = 1,
                    },
                },
            },
            ["attack_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
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
                    },
                    ["SectionOver"] = {
                        time = 0.27,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 0.967,
                        priority = 1,
                    },
                },
            },
            ["attack_3"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
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
                    },
                    ["SectionOver"] = {
                        time = 0.8,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 1.333,
                        priority = 1,
                    },
                },
            },
            ["attack_4_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
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
                    },
                },
            },
            ["attack_4_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
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
                    },
                    ["SectionOver"] = {
                        time = 0.368,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 0.833,
                        priority = 1,
                    },
                },
            },
        },
        ["EntityStateDefine.UseSkill"] = {
            ["skill_01_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
                        time = 0.3,
                        priority = 1,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [2] = {
                        name = "Hit",
                        time = 0.6,
                        priority = 2,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [3] = {
                        name = "Hit",
                        time = 0.84,
                        priority = 3,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [4] = {
                        name = "Hit",
                        time = 1.08,
                        priority = 4,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [5] = {
                        name = "Hit",
                        time = 1.32,
                        priority = 5,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 1010101,
                            },
                        },
                    },
                    ["End"] = {
                        time = 1.467,
                        priority = 1,
                    },
                },
            },
            ["skill_01_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
                        time = 0.4,
                        priority = 1,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 1010102,
                            },
                        },
                    },
                    ["SectionOver"] = {
                        time = 0.7,
                        priority = 1,
                    },
                    ["CacheBegin"] = {
                        time = 0.8,
                        priority = 99,
                    },
                    ["SectionOver"] = {
                        time = 1,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 1.133,
                        priority = 1,
                    },
                },
            },
            ["skill_02_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 1010201,
                            },
                        },
                    },
                    ["End"] = {
                        time = 0.1,
                        priority = 1,
                    },
                },
            },
            ["skill_02_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
                        time = 0.2,
                        priority = 1,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                },
                processFrame = {
                    ["Start"] = {
                        time = 1,
                        priority = 99,
                    },
                    ["PlayEffect"] = {
                        time = 0.176,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 1010202,
                            },
                        },
                    },
                    ["CacheBegin"] = {
                        time = 0.8,
                        priority = 99,
                    },
                    ["SectionOver"] = {
                        time = 1,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 1.267,
                        priority = 1,
                    },
                },
            },
            ["skill_03_1"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
                        time = 0.42,
                        priority = 1,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [2] = {
                        name = "Hit",
                        time = 0.5,
                        priority = 2,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [3] = {
                        name = "Hit",
                        time = 0.6,
                        priority = 3,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [4] = {
                        name = "Hit",
                        time = 0.7,
                        priority = 4,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [5] = {
                        name = "Hit",
                        time = 0.8,
                        priority = 4,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [6] = {
                        name = "Hit",
                        time = 0.9,
                        priority = 4,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [7] = {
                        name = "Hit",
                        time = 1,
                        priority = 4,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                    },
                    ["PlayEffect"] = {
                        time = 0.2,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 1010301,
                            },
                        },
                    },
                    ["End"] = {
                        time = 1.1,
                        priority = 1,
                    },
                },
            },
            ["skill_03_2"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
                        time = 0.4,
                        priority = 1,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 3.5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                    },
                    ["PlayEffect"] = {
                        time = 0.23,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 1010302,
                            },
                        },
                    },
                    ["CacheBegin"] = {
                        time = 1,
                        priority = 99,
                    },
                    ["SectionOver"] = {
                        time = 1.2,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 2.067,
                        priority = 1,
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
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 0.467,
                        priority = 1,
                    },
                },
            },
            ["hit_2"] = {
                iPoolType = GameConstant.POOL_ANIM_HIT,
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                    },
                    ["End"] = {
                        time = 0.467,
                        priority = 1,
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
                    },
                    ["End"] = {
                        time = 2,
                        priority = 1,
                    },
                },
            },
        },
    },
    ["FemaleWarrior"] = {
        ["EntityStateDefine.Atk"] = {
            ["2HAttack"] = {
                iPoolType = GameConstant.POOL_ANIM_ATTACK,
                keyframe = {
                    [1] = {
                        name = "Hit",
                        time = 0.3,
                        priority = 1,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [2] = {
                        name = "Hit",
                        time = 0.6,
                        priority = 2,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [3] = {
                        name = "Hit",
                        time = 0.84,
                        priority = 3,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [4] = {
                        name = "Hit",
                        time = 1.08,
                        priority = 4,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                    [5] = {
                        name = "Hit",
                        time = 1.32,
                        priority = 5,
                        data = {
                            [4] = {
                                [1] = {
                                    x = 0,
                                    y = 0,
                                    z = 0,
                                    width = 5,
                                    height = 1.7,
                                    depth = 3,
                                },
                            },
                        },
                    },
                },
                processFrame = {
                    ["Start"] = {
                        time = 0,
                        priority = 99,
                    },
                    ["PlayEffect"] = {
                        time = 0,
                        priority = 1,
                        data = {
                            [1] = {
                                type = 1,
                                id = 1010101,
                            },
                        },
                    },
                    ["End"] = {
                        time = 1.467,
                        priority = 1,
                    },
                },
            },
        },
    },
}