using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;



public class AudioController : MonoBehaviour
{
    #region private members

    [SerializeField] private List<AudioClip> listOfAudioClips;
    [SerializeField] private List<AudioClip> drumSounds;
    [SerializeField] private int seed;

    private float[] pitchValues = {  0.84085f, 0.89084f, 0.94389f, 1.0f,
        1.05947f, 1.12246f, 1.18911f, 1.25990f,
        1.33481f, 1.41416f, 1.49824f, 1.58737f };
    private List<Vector2Int> _ListOfPositionForAudioSourceOne;
    private List<Vector2Int> _ListOfPositionForAudioSourceTwo;

    private List<Component> _listOfAudioSources = new List<Component>();

    private float _timeDifference =0.0f;
    private float _sampleTimeDifference =0.5f;
    private  int _counter = 0;
    private const int AudioSourceSize = 5;
    private int StartingChord = 0;
    private enum PitchState
    { Up, Down }

    private PitchState currentPitchState = PitchState.Up;
    public int Pitch = 0;
    #endregion

    #region Private methods



    private void Start()
    {
       Array.Sort(pitchValues);
        Random.InitState(seed);
       // _ListOfPositionForAudioSourceOne = Enumerable.Repeat(new Vector2Int(-1, -1), 28 * 14).ToList();
       _ListOfPositionForAudioSourceOne = new List<Vector2Int>();
        for (int i = 0; i < AudioSourceSize; i++)
        {
            this.gameObject.AddComponent<AudioSource>();
            
        }

        GetComponents(typeof(AudioSource), _listOfAudioSources);

        SetAudioVolume();
        //GenerateAudio();
        GenerateSingleAudio();
        //DrumSettings();
        StartCoroutine(playDrumEnumerator());
     
        StartingChord = Random.Range(0, 13);
    }

    private void GenerateSingleAudio()
    {
        for (int i = 0; i < 28; i++)
        {
          
            
            StartingChord= Random.Range(0, 8);
         

            _ListOfPositionForAudioSourceOne.Add(new Vector2Int(i, StartingChord));

        }
    }

    private void DrumSettings()
    {
        var drumSourceOne = _listOfAudioSources[3] as AudioSource;
        var drumSourceTwo = _listOfAudioSources[4] as AudioSource;

        //drumSourceOne.loop = true;
        //drumSourceTwo.loop = true;
        drumSourceOne.clip = drumSounds[0];
        drumSourceTwo.clip = drumSounds[1];
        drumSourceOne.enabled = false;
        drumSourceTwo.enabled = false;
    }

    private void PlayDrumSounds()
    {
        var drumSourceOne = _listOfAudioSources[3] as AudioSource;
        var drumSourceTwo = _listOfAudioSources[4] as AudioSource;

        if (!drumSourceOne.isPlaying)
        {
            bool canPlay = Random.Range(0, 100) > 50;
            
            if (canPlay)
                drumSourceOne.enabled = true;
            else
            {
                drumSourceOne.enabled = false;
            }
        }

        if (!drumSourceTwo.isPlaying)
        {
            bool canPlay = Random.Range(0, 100) > 50;
            if (canPlay)
                drumSourceTwo.enabled = true;
            else
            {
                drumSourceTwo.enabled = false;
            }
        }
    }
    private void SetAudioVolume()
    {
        var temp = _listOfAudioSources[1] as AudioSource;
        temp.volume = 0.055f;
        temp.pitch = 1.08f;
        var temp1 = _listOfAudioSources[2] as AudioSource;
        temp1.volume = 0.035f;
        
    }

    private void GenerateAudio()
    {
      
        for (int i = 0; i < 28; i++)
        {
            var tempList = FindRandomNumList();
            for (int j = 0; j < tempList.Count; j++)
            {
               
               var id = (12*i+ tempList[j]);
               if (StartingChord == 0)
               {
                   StartingChord = 3;
               }

               if (StartingChord==14)
               {
                   StartingChord = 11;
               }

               _ListOfPositionForAudioSourceOne[id] = new Vector2Int(i,Random.Range(StartingChord-3, StartingChord+3));
            }
        }

        
    }

    private void Update()
    {
      
       // PlayDrumSounds();
       // FindAndPlaySound();
        SingleAudio(); 
       // SetTimeDifference();
    }

    private void SetTimeDifference()
    {
        if (Pitch>1.3)
        {
            _sampleTimeDifference = 0.45f;
        }
        else if(Pitch==1)
        {
            _sampleTimeDifference = 0.5f;
        }
        else
        {
            if (Pitch > 0.9)
                _sampleTimeDifference = 0.53f;
            else
            {
                _sampleTimeDifference = 0.55f;
            }
        }

    }

    private List<int> FindRandomNumList()
    {
        List<int> returnList = new List<int>();
        int rand = Random.Range(0, 4);
        for (var i = 0;i<rand;i++)
        {
            returnList.Add(Random.Range(0, listOfAudioClips.Count));
        }

        return returnList;
    }

    private void FindAndPlaySound()
    {
        if (Time.time > _sampleTimeDifference)
        {
            for (int i = _counter; i < 28; i++)
            {
                int AudioID = 0;
                int prevY = -1;
                for (int j = 0; j <14; j++)
                {
                    var id = ((12 * i )+ j);
                   var value= _ListOfPositionForAudioSourceOne[id];
                   if (value.y > -1)
                   {
                       if (AudioID<_listOfAudioSources.Count)
                       {
                       
                           PlayRandomSound(_listOfAudioSources[AudioID] as AudioSource, listOfAudioClips[value.y]);
                           AudioID++;
                       }
                       else
                       {
                           break;
                       }
                   }

                }

                _counter++;
                if (_counter == 28)
                {
                    _counter = 0;
                }

                break;
            }
        }

    }


    private void SingleAudio()
    {
        
        if (Time.time > _sampleTimeDifference)
        {
            for (int i = _counter; i < _ListOfPositionForAudioSourceOne.Count; i++)
            {
                _counter = i + 1;


                PlayRandomSound(_listOfAudioSources[0] as AudioSource, listOfAudioClips[_ListOfPositionForAudioSourceOne[i].y]);
              

                if (i + 1 == _ListOfPositionForAudioSourceOne.Count)
                {
                    _counter = 0;
                }
                break;
            }
        }
    }

    IEnumerator playDrumEnumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0,2f));
            PlayDrumSounds();
        }
    }

    private void PlayRandomSound(AudioSource audioSource, [CanBeNull] AudioClip clip)
    {
        if (clip!=null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = clip;
                audioSource.PlayDelayed(0.0f);
                //Pitch = SelectPitch();
                //audioSource.pitch = pitchValues[Pitch];
            }
        }
    }
   
    private int SelectPitch()
    {
       
        currentPitchState = Random.Range(0, 100) > 50 ? PitchState.Up: PitchState.Down;

        if (currentPitchState == PitchState.Down)
        {
            if (Pitch == 0)
            {
                Pitch = 0;
                return Pitch;
            }

            Pitch--;
        }
        else
        {
            
            if (Pitch == pitchValues.Length-1)
            {
                Pitch = 0;
                return Pitch;
            }

            Pitch++;
        }

        return Pitch;
    }

  

    #endregion
}
