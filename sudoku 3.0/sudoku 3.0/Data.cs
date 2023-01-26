namespace Datastorage
{
    public class Data
    {
        public int[,] heatmap = new int[9,9];
        public void obj()
        {
            for(int i = 0; i < 9; i++) 
            {
                for(int j = 0; j<9; i++)
                {
                    heatmap[i,j] = 0;
                }
            }
        }
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
