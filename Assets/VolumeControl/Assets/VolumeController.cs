using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public AudioSource source;
    public Slider slider;

    private void Start()
    {
          
    }

    private void Update()
    {
        source.volume = slider.value;
    }
}
