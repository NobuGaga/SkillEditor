namespace SkillEditor {

    internal struct KeyFrameData {
        public string modelName;
        public SkillClipData[] clipDatas;

        public override string ToString() {
            string clipDataString = string.Empty;
            if (clipDatas != null)
                for (int index = 0; index < clipDatas.Length; index++)
                    clipDataString += string.Format("\n[{0}] = {1}", index, clipDatas[index]);
            string toString = string.Format("clipName = {0}, frameData = \n{1}",
                                            modelName, clipDataString);
            string internString = string.Intern(toString);
            return internString;
        }
    }

    internal struct SkillClipData {
        public string clipName;
        public FrameData[] frameDatas;

        public override string ToString() {
            string frameDataString = string.Empty;
            if (frameDatas != null)
                for (int index = 0; index < frameDatas.Length; index++)
                    frameDataString += string.Format("\n[{0}] = {1}", index, frameDatas[index]);
            string toString = string.Format("clipName = {0}, frameData = \n{1}",
                                            clipName, frameDataString);
            string internString = string.Intern(toString);
            return internString;
        }
    }

    internal struct FrameData {
        public string name;
        public float time;
        public short priority;
        public CustomData[] customData;

        public override string ToString() {
            string customDataString = string.Empty;
            if (customData != null)
                for (int index = 0; index < customData.Length; index++)
                    customDataString += string.Format("\n[{0}] = {1}", index, customData[index]);
            string toString = string.Format("name = {0}, time = {1}, priority = {2}\ncustomData = {3}",
                                            name, time, priority, customDataString);
            string internString = string.Intern(toString);
            return internString;
        }
    }

    internal struct CustomData {
        public EFrameType frameType;
        public object data;

        public override string ToString() {
            string dataString = string.Empty;
            if (data is GrabData)
                dataString = ((GrabData)data).ToString();
            else if (data is UngrabData)
                dataString = ((UngrabData)data).ToString();
            else if (data is HarmData[]) {
                HarmData[] array = data as HarmData[];
                for (int index = 0; index < array.Length; index++)
                    dataString += string.Format("\n[{0}] = {1}", index, array[index]);
            }
            string toString = string.Format("frameType = {0}, data = {1}", frameType, dataString);
            string internString = string.Intern(toString);
            return internString;
        }
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

        public override string ToString() {
            string toString = string.Format("type = {0}, id = {1}", type, id);
            string internString = string.Intern(toString);
            return internString;
        }
    }

    internal struct UngrabData {
        public short type;
        public int id;

        public override string ToString() {
            string toString = string.Format("type = {0}, id = {1}", type, id);
            string internString = string.Intern(toString);
            return internString;
        }
    }

    internal struct HarmData {
        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public short depth;

        public override string ToString() {
            string toString = string.Format("x = {0}, y = {1}, z = {2}, width = {3}, height = {4}, depth = {5}",
                                            x, y, z, width, height, depth);
            string internString = string.Intern(toString);
            return internString;
        }
    }
}