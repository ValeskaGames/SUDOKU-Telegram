namespace Datastorage
{
    public class Data
    {
        public int[,] heatmap = new int[9,9];
        public void Add(int i, int j)
        {
            heatmap[i, j]++;
        }
        public int Get(int i, int j)
        {
            return heatmap[i, j];
        }
    }
}
