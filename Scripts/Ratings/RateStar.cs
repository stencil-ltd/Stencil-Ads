using Binding;
using UnityEngine;
using UnityEngine.UI;

namespace Ratings
{
    public class RateStar : MonoBehaviour
    {
        public int Index { get; set; }
        public Image Fill;

        public Button Button => GetComponent<Button>();
    }
}