using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    public AudioSource audioSource;
    // public float _maxScale = 1;
    // public float _minScale = 0.5f;

    float[] _samples = new float[512];
    float[] _freqBand = new float[8];
    float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];
    float[] _freqBandHeighest = new float[8];
    // public float[] _audioBand = new float[8];
    public float[] _audioBandBuffer = new float[8];

    public int _bandIndex = 3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float[] spectrum = new float[256];

        AudioListener.GetSpectrumData(_samples, 0, FFTWindow.Rectangular);
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();

        // float spectrumRatio = _audioBandBuffer[_bandIndex] > 0 ? _audioBandBuffer[_bandIndex] / 2f : 0f;

        // float spectrumScale = Mathf.Lerp(_minScale, _maxScale, spectrumRatio);

        // transform.localScale = new Vector3(spectrumScale, spectrumScale, spectrumScale);

    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHeighest[i])
            {
                _freqBandHeighest[i] = _freqBand[i];
            }
            // _audioBand[i] = (_freqBand[i] / _freqBandHeighest[i]);
            _audioBandBuffer[i] = (_freqBand[i] / _freqBandHeighest[i]);
        }
    }


    void BandBuffer()
    {
        for (int i = 0; i < 8; ++i)
        {
            if (_freqBand[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _freqBand[i];
                _bufferDecrease[i] = 0.005f;
            }

            if (_freqBand[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count * 1);
                count++;
            }

            average /= count;

            _freqBand[i] = average * 10;
        }
    }
}
