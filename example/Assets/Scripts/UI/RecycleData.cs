namespace UI
{
    public class RecycleData
    {
        public int Index { get; }

        public RecycleData(int index) => Index = index;

        public object Data { get; set; }
    }
}