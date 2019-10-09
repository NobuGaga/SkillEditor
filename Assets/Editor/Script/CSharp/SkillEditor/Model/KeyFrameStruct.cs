namespace SkillEditor {

    internal struct KeyFrameData {
        public string modelName;
        public SkillClipData clipData;
    }

    internal struct SkillClipData {
        public FrameData[] frameDatas;
    }

    internal struct FrameData {
        public string name;
        public float time;
        public short priority;
        public CustomData customData;
    }

    internal struct CustomData {
        public EFrameType frameType;
        public object data;
    }

    internal enum EFrameType {
        None = 0,
        Grab = 1,
        Ungrab = 2,
        Collision = 3,
        Harm = 4,
        Max = 5,
    }

    internal struct GrabData {
        public short type;
        public int id;
    }

    internal struct UngrabData {
        public short type;
        public int id;
    }

    internal struct HarmData {
        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public short depth;
    }
}