using System.Collections.Generic;
using UI;

namespace MemoryScene
{
    public class MemorySceneManager : BaseScene
    {
        public RecycleGridView gridView;
        
        protected void Start()
        {
            gridView.OnEndDrag(OnEndDrag);
            GetList();
        }

        private void OnEndDrag()
        {
            GetList();
        }
        
        //create mock data
        private void GetList()
        {
            var list = new List<RecycleData>();
            for (var i = 0; i < 100; i++)
            {
                var cardData = new MockCardData()
                {
                    Id = i,
                    Title = $"Title{i}",
                    Url = $"https://images.unsplash.com/photo-1575936123452-b67c3203c357?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1170&q=80"
                };
                
                list.Add(new RecycleData(i)
                {
                    Data = cardData
                });
            }
            
            gridView.UpdateContents(list);
        }
    }
}
