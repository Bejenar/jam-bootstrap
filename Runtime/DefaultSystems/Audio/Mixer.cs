using UnityEngine.Audio;

namespace _Project.Data
{
    public class Mixer : CMSEntity
    {
        public Mixer()
        {
            Define<TagMixer>().mixer = "Mixer".Load<AudioMixer>();
        }
    }

    public class TagMixer : EntityComponentDefinition
    {
        public AudioMixer mixer;
    }
}