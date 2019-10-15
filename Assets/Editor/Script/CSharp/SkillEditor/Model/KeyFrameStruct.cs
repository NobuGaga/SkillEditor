namespace SkillEditor.Structure {

    using LuaTableKeyValue = LuaFormat.LuaTableKeyValue;

    internal enum KeyFrameLuaLayer {
        EnterTable = 0,
        Model = 1,
        Clip = 2,
        FrameIndex = 3,
        FrameData = 4,
        CustomData = 5,
        Effect = 6,
        Rect = 7,
    }

    internal struct KeyFrameData {
        public string modelName;
        public SkillClipData[] clipDatas;

        public KeyFrameData(string modelName, SkillClipData[] clipDatas) {
            this.modelName = modelName;
            this.clipDatas = clipDatas;
        }

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

        public SkillClipData(string clipName, FrameData[] frameDatas) {
            this.clipName = clipName;
            this.frameDatas = frameDatas;
        }

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

    internal struct FrameData : ITable {
        public int frameIndex;
        public string name;
        public float time;
        public short priority;
        public CustomData[] customData;

        public FrameData(int frameIndex, string name, float time, short priority, CustomData[] customData) {
            this.frameIndex = frameIndex;
            this.name = name;
            this.time = time;
            this.priority = priority;
            this.customData = customData;
        }

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

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Name:
                    name = value as string;
                    return;
                case Key_Time:
                    time = (float)value;
                    return;
                case Key_Priority:
                    priority = (short)(int)value;
                    return;
                case Key_Data:
                    customData = value as CustomData[];
                    return;
            }
        }

        private const string Key_Name = "name";
        private const string Key_Time = "time";
        private const string Key_Priority = "priority";
        private const string Key_Data = "data";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[4];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Name, LuaFormat.Type.LuaString);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Time, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[2] = new LuaTableKeyValue(Key_Priority, LuaFormat.Type.LuaInt);
            m_arraykeyValue[3] = new LuaTableKeyValue(Key_Data, LuaFormat.Type.LuaTable);
            return m_arraykeyValue;
        }
    }

    internal struct CustomData {
        public EFrameType frameType;
        public object data;

        public CustomData(EFrameType frameType, object data) {
            this.frameType = frameType;
            this.data = data;
        }

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

    internal struct GrabData : ITable {
        public short type;
        public int id;

        public GrabData(short type, int id) {
            this.type = type;
            this.id = id;
        }

        public override string ToString() {
            string toString = string.Format("type = {0}, id = {1}", type, id);
            string internString = string.Intern(toString);
            return internString;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    type = (short)(int)value;
                    return;
                case Key_Id:
                    id = (short)(int)value;
                    return;
            }
        }

        private const string Key_Type = "type";
        private const string Key_Id = "id";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[2];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Type, LuaFormat.Type.LuaInt);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Id, LuaFormat.Type.LuaInt);
            return m_arraykeyValue;
        }
    }

    internal struct UngrabData : ITable {
        public short type;
        public int id;

        public UngrabData(short type, int id) {
            this.type = type;
            this.id = id;
        }

        public override string ToString() {
            string toString = string.Format("type = {0}, id = {1}", type, id);
            string internString = string.Intern(toString);
            return internString;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    type = (short)(int)value;
                    return;
                case Key_Id:
                    id = (short)(int)value;
                    return;
            }
        }

        private const string Key_Type = "type";
        private const string Key_Id = "id";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[2];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Type, LuaFormat.Type.LuaInt);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Id, LuaFormat.Type.LuaInt);
            return m_arraykeyValue;
        }
    }

    internal struct HarmData : ITable {
        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public short depth;

        public HarmData(float x, float y, float z, float width, float height, short depth) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public override string ToString() {
            string toString = string.Format("x = {0}, y = {1}, z = {2}, width = {3}, height = {4}, depth = {5}",
                                            x, y, z, width, height, depth);
            string internString = string.Intern(toString);
            return internString;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_X:
                    x = (float)value;
                    return;
                case Key_Y:
                    y = (float)value;
                    return;
                case Key_Z:
                    z = (float)value;
                    return;
                case Key_Width:
                    width = (float)value;
                    return;
                case Key_Height:
                    height = (float)value;
                    return;
                case Key_Depth:
                    depth = (short)(int)value;
                    return;
            }
        }

        private const string Key_X = "x";
        private const string Key_Y = "y";
        private const string Key_Z = "z";
        private const string Key_Width = "width";
        private const string Key_Height = "height";
        private const string Key_Depth = "depth";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[6];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_X, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Y, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[2] = new LuaTableKeyValue(Key_Z, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[3] = new LuaTableKeyValue(Key_Width, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[4] = new LuaTableKeyValue(Key_Height, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[5] = new LuaTableKeyValue(Key_Depth, LuaFormat.Type.LuaInt);
            return m_arraykeyValue;
        }
    }

    internal interface ITable {

        void SetTableKeyValue(string key, object value);
        LuaTableKeyValue[] GetTableKeyValueList();
    }
}