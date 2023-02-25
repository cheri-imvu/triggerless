using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.XAFLib
{
    public class VocalEntry
    {
        public VocalShape Shape { get; set; }
        public float StartsAt { get; set; }
    }

    public struct VocalShape
    {
        public float Open { get; set; }
        public float Smile { get; set; }
        public float Pucker { get; set; }

        public static VocalShape OO => new VocalShape { Open = 0.182f, Pucker = 0.787f, Smile = 0 };
        public static VocalShape OH => new VocalShape { Open = 0.322f, Pucker = 0.682f, Smile = 0 };
        public static VocalShape MM => new VocalShape { Open = 0, Pucker = 0, Smile = 0 };
        public static VocalShape AH => new VocalShape { Open = 0.322f, Pucker = 0.457f, Smile = 0.069f };
        public static VocalShape EH => new VocalShape { Open = 0.224f, Pucker = 0.252f, Smile = 0.12f };
        public static VocalShape AY => new VocalShape { Open = 0.224f, Pucker = 0.066f, Smile = 0.19f };
        public static VocalShape EE => new VocalShape { Open = 0.104f, Pucker = 0.066f, Smile = 0.357f };

    }

    public class MorphAnimationService
    {
        public MorphAnimation Convert(List<VocalEntry> list)
        {
            MorphAnimation result = new MorphAnimation();
            var trackOpen = new MorphTrack();
            var trackPucker = new MorphTrack();
            var trackSmile = new MorphTrack();

            trackOpen.Name = MorphAnimation.NamesMap["mop"];
            trackPucker.Name = MorphAnimation.NamesMap["mpk"];
            trackSmile.Name = MorphAnimation.NamesMap["msc"];

            var initFrame = new MorphKeyFrame { Time = 0, Weight = 0 };
            trackOpen.KeyFrames.Add(initFrame);
            trackPucker.KeyFrames.Add(initFrame);
            trackSmile.KeyFrames.Add(initFrame);

            float currentTime = 0;
            foreach (var entry in list)
            {
                currentTime = entry.StartsAt;
                var frameOpen = new MorphKeyFrame { Time = entry.StartsAt, Weight = entry.Shape.Open };
                var framePucker = new MorphKeyFrame { Time = entry.StartsAt, Weight = entry.Shape.Pucker };
                var frameSmile = new MorphKeyFrame { Time = entry.StartsAt, Weight = entry.Shape.Smile };

                trackOpen.KeyFrames.Add(frameOpen);
                trackPucker.KeyFrames.Add(framePucker);
                trackSmile.KeyFrames.Add(frameSmile);
            }

            result.Duration = currentTime;
            result.Tracks.Add(trackOpen);
            result.Tracks.Add(trackPucker);
            result.Tracks.Add(trackSmile);

            return result;

        }
    }
}
