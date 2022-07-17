using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioAnalysis {
    static float[] samples = new float[1024];

    static float samplingFrequency = AudioSettings.outputSampleRate;

    public static float[] GetSamples(AudioSource audioSource) {
        audioSource.GetOutputData(samples, 0);
        return samples;
    }

    public static float[] GetSpectrum(AudioSource audioSource) {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
        return samples;
    }

    public static float MeanEnergy(AudioSource audioSource) {
        audioSource.GetOutputData(samples, 0);
        float sum = 0;
        for (int i = 0; i < samples.Length; i++) {
            sum += samples[i] * samples[i];
        }
        return sum / samples.Length;
    }

    public static float ComputeRMS(AudioSource audioSource) {
        float meanEnergy = MeanEnergy(audioSource);
        return Mathf.Sqrt(meanEnergy);
    }

    public static float ComputeSpectrumPeak(AudioSource audioSource, bool interpolate) {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
        float max = 0;
        float index = 0;
        for (int i = 0; i < 3 * samples.Length / 4; i++) {
            if (samples[i] > max) {
                max = samples[i];
                index = i;
            }
        }

        if (interpolate && index > 0 && index < samples.Length - 1) {
            float dL = samples[(int)index] - samples[(int)index - 1];
            float dR = samples[(int)index] - samples[(int)index + 1];
            index += (dL - dR) / (2 * (dL + dR));
        }
        float peak = index * 0.5f * samplingFrequency / samples.Length;
        return peak;
    }
    public static float ConcentrationAroundPeak(float peakFrequency) {
        int index = (int)(peakFrequency * samples.Length / (0.5f * samplingFrequency));
        int centrdBand = samples.Length / 200;
        float total = 0;
        float insideBand = 0;
        for (int i = 0; i < samples.Length - 5; i++) {
            total += samples[i] * samples[i];
            if (Mathf.Abs(i - index) < centrdBand)
                insideBand += samples[i] * samples[i];
        }

        return insideBand / total;

    }

    public static float ConvertToDB(float val) {
        float reference = 1e-7f;
        if (val < reference) val = reference;
        return 10 * Mathf.Log10(val / reference);
    }

    public static float[] FreqBands(AudioSource audioSource) {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
        int nbands = 7;
        float[] bands = new float[nbands];
        int numSamples = 8 * samples.Length / 1024;
        int iniSample = 0;
        for (int i = 0; i < nbands; i++) {
            float val = 0;
            for (int j = iniSample; j < iniSample + numSamples; j++) {
                val += samples[j] * samples[j];
            }
            bands[i] = ConvertToDB(val / numSamples);
            iniSample += numSamples;
            numSamples *= 2;
        }
        return bands;
    }

    public static bool Whistle(AudioSource audioSource)
    {
        float energy = AudioAnalysis.MeanEnergy(audioSource);

        if (AudioAnalysis.ConvertToDB(energy) > 40)
        {

            float peakFrequency = AudioAnalysis.ComputeSpectrumPeak(audioSource, true);
            float concentration = AudioAnalysis.ConcentrationAroundPeak(peakFrequency);

            //assombio
            if (concentration > 0.8f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
