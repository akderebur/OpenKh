using OpenKh.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xe.BinaryMapper;

namespace OpenKh.Kh2.Ard
{
    public static class Event
    {
        private static readonly Dictionary<int, Func<Stream, IEventEntry>> _entryType = new Dictionary<int, Func<Stream, IEventEntry>>()
        {
            [0x00] = EntryProject.Read,
            [0x01] = EntryActor.Read,
            [0x02] = EntryActorPosition.Read,
            [0x03] = EntryMap.Read,
            [0x06] = EntryCameraTimeline.Read,
            [0x08] = EntryUnk08.Read,
            [0x0A] = EntryUnk0A.Read,
            [0x0C] = EntryUnk0C.Read,
            [0x0F] = EntryUnk0F.Read,
            [0x10] = EntryFadeIn.Read,
            [0x12] = EntryFade.Read,
            [0x13] = EntryCamera.Read,
            [0x14] = EntryUnk14.Read,
            [0x1A] = EntryUnk1A.Read,
            [0x24] = EntryLoadAssets.Read,
            [0x2A] = EntryUnk2A.Read,
            [0x2B] = EntryPlayBgm.Read,
            [0x2C] = EntryRunAnimation.Read,
            [0x2D] = EntryDialog.Read,
            [0x2E] = EntryUnk2E.Read,
            [0x2F] = EntryUnk2F.Read,
            [0x30] = EntryUnk30.Read,
        };

        private static readonly Dictionary<int, Func<Stream, IEventLoad>> _loadType = new Dictionary<int, Func<Stream, IEventLoad>>()
        {
            [0x25] = LoadAnimation.Read,
            [0x38] = LoadObject.Read,
        };

        public interface IEventEntry
        { }

        public interface IEventLoad
        { }

        public class EntryProject : IEventEntry // unused
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }
            public int Unk04 { get; set; }
            public string Name { get; set; }

            private EntryProject(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                Unk04 = stream.ReadInt16();
                Name = ReadCStyleString(stream);
            }

            public override string ToString() =>
                $"Project: {Name} ({Unk00:X}, {Unk02:X}, {Unk04:X})";

            public static IEventEntry Read(Stream stream) => new EntryProject(stream);
        }

        public class EntryActor : IEventEntry // sub_22F528
        {
            public int ObjectEntry { get; set; }
            public int ActorId { get; set; }
            public string Name { get; set; }

            private EntryActor(Stream stream)
            {
                ObjectEntry = stream.ReadInt16();
                ActorId = stream.ReadInt16();
                Name = ReadCStyleString(stream);
            }

            public override string ToString() =>
                $"Actor: ObjEntry {ObjectEntry:X}, Name {Name}, ActorID {ActorId}";

            public static IEventEntry Read(Stream stream) => new EntryActor(stream);
        }
        
        public class EntryActorPosition : IEventEntry
        {
            public float Unk00 { get; set; }
            public float PositionX { get; set; }
            public float PositionY { get; set; }
            public float PositionZ { get; set; }
            public float Unk10 { get; set; }
            public float Rotation { get; set; }
            public float Unk18 { get; set; }
            public float Unk1C { get; set; }
            public int ActorId { get; set; }
            public int Unk22 { get; set; }

            private EntryActorPosition(Stream stream)
            {
                Unk00 = stream.ReadSingle();
                PositionX = stream.ReadSingle();
                PositionY = stream.ReadSingle();
                PositionZ = stream.ReadSingle();
                Unk10 = stream.ReadSingle();
                Rotation = stream.ReadSingle();
                Unk18 = stream.ReadSingle();
                Unk1C = stream.ReadSingle();
                ActorId = stream.ReadInt16();
                Unk22 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"ActorPosition: ActorID {ActorId}, Pos({PositionX}, {PositionY}, {PositionZ}) Rot({Rotation}) Unk({Unk00}, {Unk10}, {Unk18}, {Unk1C}, {Unk22})";

            public static IEventEntry Read(Stream stream) => new EntryActorPosition(stream);
        }

        public class EntryMap : IEventEntry // unused
        {
            public int Place { get; set; }
            public string World { get; set; }

            private EntryMap(Stream stream)
            {
                Place = stream.ReadInt16();
                World = ReadCStyleString(stream);
            }

            public override string ToString() =>
                $"Map: {World}{Place:D02}";

            public static IEventEntry Read(Stream stream) => new EntryMap(stream);
        }

        public class EntryFadeIn : IEventEntry // sub_22D3A8
        {
            public int FadeIn { get; set; } // dword_35DE40

            private EntryFadeIn(Stream stream)
            {
                FadeIn = stream.ReadInt16();
                stream.ReadInt16();
            }

            public override string ToString() =>
                $"FadeIn: {FadeIn} frames";

            public static IEventEntry Read(Stream stream) => new EntryFadeIn(stream);
        }

        public class EntryFade: IEventEntry
        {
            public enum FadeType
            {
                FromBlack,
                ToBlack,
                ToWhite,
                FromBlackVariant,
                ToBlackVariant,
                FromWhite,
                ToWhiteVariant,
                FromWhiteVariant,
            }

            public int FrameIndex { get; set; }
            public int Duration { get; set; }
            public FadeType Type { get; set; }

            private EntryFade(Stream stream)
            {
                FrameIndex = stream.ReadInt16();
                Duration = stream.ReadInt16();
                Type = (FadeType)stream.ReadInt16();
            }

            public override string ToString() =>
                $"Fade: Frame {FrameIndex}, Duration {Duration}, {Type}";

            public static IEventEntry Read(Stream stream) => new EntryFade(stream);
        }

        public class EntryUnk14 : IEventEntry
        {
            public int Unk00 { get; set; }

            private EntryUnk14(Stream stream)
            {
                Unk00 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk14: {Unk00}";

            public static IEventEntry Read(Stream stream) => new EntryUnk14(stream);
        }
        
        public class EntryUnk1A : IEventEntry
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }

            private EntryUnk1A(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk1A: {Unk00} {Unk02}";

            public static IEventEntry Read(Stream stream) => new EntryUnk1A(stream);
        }

        public class EntryCameraTimeline : IEventEntry // ignored
        {
            public int CameraId { get; set; }
            public int FrameStart { get; set; }
            public int FrameEnd { get; set; }
            public int Unk06 { get; set; }

            private EntryCameraTimeline(Stream stream)
            {
                CameraId = stream.ReadInt16();
                FrameStart = stream.ReadInt16();
                FrameEnd = stream.ReadInt16();
                Unk06 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"CameraTimeline: CameraID {CameraId}, Frame start {FrameStart}, Frame end {FrameEnd}, {Unk06}";

            public static IEventEntry Read(Stream stream) => new EntryCameraTimeline(stream);
        }

        public class EntryUnk08 : IEventEntry // sub_22D3B8
        {
            public int Unk00 { get; set; } // dword_35DE28
            public int Unk02 { get; set; } // ignored

            private EntryUnk08(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk08: {Unk00} {Unk02}";

            public static IEventEntry Read(Stream stream) => new EntryUnk08(stream);
        }

        public class EntryUnk0A : IEventEntry // unused
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }
            public int Unk04 { get; set; }
            public short ActorId { get; set; }
            public short WeaponId { get; set; }
            public short Unk0A { get; set; }
            public short Unk0C { get; set; }

            private EntryUnk0A(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                Unk04 = stream.ReadInt16();
                ActorId = stream.ReadInt16();
                WeaponId = stream.ReadInt16();
                Unk0A = stream.ReadInt16();
                Unk0C = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk0A: {Unk00}, {Unk02}, ActorID {ActorId}, WeaponID {WeaponId}, {Unk0A}, {Unk0C}";

            public static IEventEntry Read(Stream stream) => new EntryUnk0A(stream);
        }

        public class EntryUnk0C : IEventEntry // ignored
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }
            public int Unk04 { get; set; }
            public int Unk06 { get; set; }
            public int Unk08 { get; set; }
            public int Unk0A { get; set; }

            private EntryUnk0C(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                Unk04 = stream.ReadInt16();
                Unk06 = stream.ReadInt16();
                Unk08 = stream.ReadInt16();
                Unk0A = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk0C: {Unk00} {Unk02} {Unk04} {Unk06} {Unk08} {Unk0A}";

            public static IEventEntry Read(Stream stream) => new EntryUnk0C(stream);
        }

        public class EntryUnk0F : IEventEntry // sub_22d358
        {
            private EntryUnk0F(Stream stream)
            {
            }

            public override string ToString() =>
                $"Unk0F";

            public static IEventEntry Read(Stream stream) => new EntryUnk0F(stream);
        }

        public class EntryCamera : IEventEntry
        {
            private class Header
            {
                [Data] public short Count { get; set; }
                [Data] public short Index { get; set; }
            }

            public class CameraValue
            {
                [Data] public float Speed { get; set; }
                [Data] public float Value { get; set; }
                [Data] public float Unk08 { get; set; }
                [Data] public float Unk0C { get; set; }

                public override string ToString() =>
                    $"Value {Value}, Speed {Speed}, {Unk08}, {Unk0C}";
            }

            public int CameraId { get; set; }
            public List<CameraValue> PositionX { get; set; }
            public List<CameraValue> PositionY { get; set; }
            public List<CameraValue> PositionZ { get; set; }
            public List<CameraValue> Channel3 { get; set; }
            public List<CameraValue> Channel4 { get; set; }
            public List<CameraValue> Channel5 { get; set; }
            public List<CameraValue> Channel6 { get; set; }
            public List<CameraValue> Channel7 { get; set; }

            private EntryCamera(Stream stream)
            {
                CameraId = stream.ReadInt32();
                var headers = Enumerable
                    .Range(0, 8)
                    .Select(x => BinaryMapping.ReadObject<Header>(stream))
                    .ToList();
                var valueCount = headers.Max(x => x.Index + x.Count);
                var values = Enumerable
                    .Range(0, valueCount)
                    .Select(x => BinaryMapping.ReadObject<CameraValue>(stream))
                    .ToList();

                PositionX = AssignValues(headers[0], values);
                PositionY = AssignValues(headers[1], values);
                PositionZ = AssignValues(headers[2], values);
                Channel3 = AssignValues(headers[3], values);
                Channel4 = AssignValues(headers[4], values);
                Channel5 = AssignValues(headers[5], values);
                Channel6 = AssignValues(headers[6], values);
                Channel7 = AssignValues(headers[7], values);
            }

            private static List<CameraValue> AssignValues(Header header, IList<CameraValue> values) =>
                Enumerable.Range(0, header.Count).Select(i => values[header.Index + i]).ToList();

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Camera: ID {CameraId}");
                sb.AppendLine($"\tPositionX: {ToString(PositionX)}");
                sb.AppendLine($"\tPositionY: {ToString(PositionY)}");
                sb.AppendLine($"\tPositionZ: {ToString(PositionZ)}");
                sb.AppendLine($"\tChannel3: {ToString(Channel3)}");
                sb.AppendLine($"\tChannel4: {ToString(Channel4)}");
                sb.AppendLine($"\tChannel5: {ToString(Channel5)}");
                sb.AppendLine($"\tChannel6: {ToString(Channel6)}");
                sb.AppendLine($"\tChannel7: {ToString(Channel7)}");
                return sb.ToString();
            }

            private string ToString(IList<CameraValue> values)
            {
                if (values.Count == 1)
                    return values[0].ToString();
                return string.Join("\n\t\t", values.Select(x => x.ToString()));
            }

            public static IEventEntry Read(Stream stream) => new EntryCamera(stream);
        }

        public class EntryLoadAssets : IEventEntry
        {
            public int Unk02 { get; set; }
            public int Unk04 { get; set; }
            public int Unk06 { get; set; }
            public List<IEventLoad> Loads { get; set; }

            private EntryLoadAssets(Stream stream)
            {
                var itemCount = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                Unk04 = stream.ReadInt16();
                Unk06 = stream.ReadInt16();

                Loads = new List<IEventLoad>();
                for (var i = 0; i < itemCount; i++)
                {
                    var startPosition = stream.Position;
                    var type = stream.ReadInt16();
                    var blockLength = stream.ReadInt16();
                    if (_loadType.TryGetValue(type, out var read))
                        Loads.Add(read(stream));
                    stream.Position = startPosition + blockLength;
                }
            }

            public override string ToString() =>
                $"LoadAssets: {string.Join("\n\t", Loads.Select(x => x.ToString()))}";

            public static IEventEntry Read(Stream stream) => new EntryLoadAssets(stream);
        }

        public class EntryUnk2A : IEventEntry // sub_232A48
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }
            public int Unk04 { get; set; }

            private EntryUnk2A(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                Unk04 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk2A: {Unk00} {Unk02} {Unk04}";

            public static IEventEntry Read(Stream stream) => new EntryUnk2A(stream);
        }

        public class EntryPlayBgm : IEventEntry
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }
            public int Unk04 { get; set; }
            public int Unk06 { get; set; }
            public int FrameStart { get; set; }
            public int Unk0A { get; set; }

            private EntryPlayBgm(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                Unk04 = stream.ReadInt16();
                Unk06 = stream.ReadInt16();
                FrameStart = stream.ReadInt16();
                Unk0A = stream.ReadInt16();
            }

            public override string ToString() =>
                $"PlayBGM: {Unk00}, {Unk02}, {Unk04}, {Unk06}, Frame {FrameStart}, {Unk0A}";

            public static IEventEntry Read(Stream stream) => new EntryPlayBgm(stream);
        }

        public class EntryRunAnimation : IEventEntry
        {
            public int FrameStart { get; set; }
            public int FrameEnd { get; set; }
            public int Unk06 { get; set; }
            public int Unk08 { get; set; }
            public int ActorId { get; set; }
            public int Unk0C { get; set; }
            public string Path { get; set; }

            private EntryRunAnimation(Stream stream)
            {
                FrameStart = stream.ReadInt16();
                FrameEnd = stream.ReadInt32();
                Unk06 = stream.ReadInt16();
                Unk08 = stream.ReadInt16();
                ActorId = stream.ReadInt16();
                Unk0C = stream.ReadInt16();
                Path = ReadCStyleString(stream);
            }

            public override string ToString() =>
                $"RunAnimation: Frame start {FrameStart}, Frame end {FrameEnd}, {Unk06}, {Unk08}, ActorID {ActorId}, {Unk0C}, {Path}";

            public static IEventEntry Read(Stream stream) => new EntryRunAnimation(stream);
        }

        public class EntryDialog : IEventEntry
        {
            public int FrameIndex { get; set; }
            public int Unk02 { get; set; }
            public int MessageId { get; set; }
            public int Unk06 { get; set; }
            public int Unk08 { get; set; }

            private EntryDialog(Stream stream)
            {
                FrameIndex = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                MessageId = stream.ReadInt16();
                Unk06 = stream.ReadInt16();
                Unk08 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Dialog: Frame index {FrameIndex}, {Unk02}, MsgID {MessageId}, {Unk06}, {Unk08}";

            public static IEventEntry Read(Stream stream) => new EntryDialog(stream);
        }

        public class EntryUnk2E : IEventEntry
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }
            public int Unk04 { get; set; }

            private EntryUnk2E(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                Unk04 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk2E: {Unk00} {Unk02} {Unk04}";

            public static IEventEntry Read(Stream stream) => new EntryUnk2E(stream);
        }

        public class EntryUnk2F : IEventEntry
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }

            private EntryUnk2F(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk2F: {Unk00} {Unk02}";

            public static IEventEntry Read(Stream stream) => new EntryUnk2F(stream);
        }

        public class EntryUnk30 : IEventEntry
        {
            public int Unk00 { get; set; }
            public int Unk02 { get; set; }
            public int Unk04 { get; set; }

            private EntryUnk30(Stream stream)
            {
                Unk00 = stream.ReadInt16();
                Unk02 = stream.ReadInt16();
                Unk04 = stream.ReadInt16();
            }

            public override string ToString() =>
                $"Unk30: {Unk00} {Unk02} {Unk04}";

            public static IEventEntry Read(Stream stream) => new EntryUnk30(stream);
        }

        public class LoadObject : IEventLoad
        {
            public int ObjectId { get; set; }
            public int ActorId { get; set; }
            public string Name { get; set; }

            private LoadObject(Stream stream)
            {
                ObjectId = stream.ReadInt16();
                ActorId = stream.ReadInt16();
                Name = ReadCStyleString(stream);
            }

            public override string ToString() =>
                $"Object: ObjectEntry {ObjectId:X04}, Name {Name}, ActorID {ActorId}";

            public static IEventLoad Read(Stream stream) => new LoadObject(stream);
        }

        public class LoadAnimation : IEventLoad
        {
            public int ObjectId { get; set; }
            public int ActorId { get; set; }
            public int UnknownIndex { get; set; }
            public string Name { get; set; }

            private LoadAnimation(Stream stream)
            {
                ObjectId = stream.ReadInt16();
                ActorId = stream.ReadInt16();
                UnknownIndex = stream.ReadInt16();
                Name = ReadCStyleString(stream);
            }

            public override string ToString() =>
                $"Animation: ObjectEntry {ObjectId:X04}, ActorID {ActorId}, Unk? {UnknownIndex}, Path {Name}";

            public static IEventLoad Read(Stream stream) => new LoadAnimation(stream);
        }

        public static List<IEventEntry> Read(Stream stream)
        {
            var entries = new List<IEventEntry>();

            int blockLength;
            while ((blockLength = stream.ReadInt16()) > 0)
            {
                var startPosition = stream.Position - 2;
                var type = stream.ReadInt16();
                entries.Add(_entryType[type](stream));

                if (stream.Position != startPosition + blockLength)
                    stream.Position = stream.Position;
                stream.Position = startPosition + blockLength;
            }

            var str = string.Join("\n", entries.Select(x => x.ToString()));
            return entries;
        }

        private static string ReadCStyleString(Stream stream)
        {
            var sb = new StringBuilder();
            while (stream.Position < stream.Length)
            {
                var ch = stream.ReadByte();
                if (ch == 0)
                    break;

                sb.Append((char)ch);
            }

            return sb.ToString();
        }
    }
}
