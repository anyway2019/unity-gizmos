using FancyScrollView;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryScene
{
    public class MockCardData
    {
        public int Id { get; set; }
            
        public string Title { get; set; }
            
        public string Url { get; set; }
    }
    
    public class Card : FancyGridViewCell<RecycleData, RecycleContext>
    {
        [SerializeField] Text title = default;
        [SerializeField] Image image = default;
        [SerializeField] Button button = default;
        private MemorySceneManager _manager;

        private MemorySceneManager Manager
        {
            get
            {
                if (_manager == null)
                    _manager = FindObjectOfType<MemorySceneManager>();
                return _manager;
            }
        }

        protected void Start()
        {
            _manager = FindObjectOfType<MemorySceneManager>();
        }

        public override void Initialize()
        {
            button.onClick.AddListener(() => Context.OnCellClicked?.Invoke(Index));
        }

        public override void UpdateContent(RecycleData itemData)
        {
            if (itemData.Data is not MockCardData data) return;
            
            title.text = data.Title;
            Manager.ShowImage(image, data.Url);
        }
        
        public override void Dispose()
        {
            image = null;
        }
    }
}