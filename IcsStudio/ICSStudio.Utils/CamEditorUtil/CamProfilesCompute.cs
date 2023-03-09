using System.Collections.Generic;

namespace ICSStudio.Utils.CamEditorUtil
{
        /// <summary>
        ///     用于CamProfiles的计算，精度上有差异，后续考虑用double数据类型
        /// </summary>
        public class CamProfilesCompute
        {
            /// <summary>
            /// </summary>
            /// <param name="profiles"></param>
            /// <param name="startSlope"></param>
            /// <param name="endSlope"></param>
            public static void Compute(List<CamProfile> profiles, double startSlope = 0,
                double endSlope = 0)
            {
                if (profiles.Count == 0)
                    return;

                if (profiles.Count == 1)
                {
                    var profile = profiles[0];
                    profile.Status = 2;
                    profile.C0 = 0;
                    profile.C1 = 0;
                    profile.C2 = 0;
                    profile.C3 = 0;

                    return;
                }

                var cubicList = new List<CamProfile>();
                cubicList.Clear();

                var cubicStartSlop = startSlope;
                double cubicEndSlop;

                for (var i = 0; i < profiles.Count; i++)
                {
                    var profile = profiles[i];

                    if (profile.Type == SegmentType.Linear)
                    {
                        profile.Status = 2;
                        profile.C0 = 0;
                        profile.C2 = 0;
                        profile.C3 = 0;
                        // last one
                        if (i == profiles.Count - 1)
                        {
                            profile.C1 = endSlope;
                        }
                        else
                        {
                            var nextProfile = profiles[i + 1];
                            profile.C1 = (nextProfile.Y - profile.Y) / (nextProfile.X - profile.X);
                        }

                        if (cubicList.Count > 0)
                        {
                            cubicList.Add(profile);
                            cubicEndSlop = profile.C1;

                            ComputeCubic(cubicList, cubicStartSlop, cubicEndSlop);

                            cubicList.Clear();
                        }


                        // update cubic startSlop
                        cubicStartSlop = profile.C1;
                    }

                    //当Type不等于直线时，其他情况全都视为曲线来显示
                    if (profile.Type != SegmentType.Linear)
                    {
                        cubicList.Add(profile);
                    }
                }

                if (cubicList.Count > 0)
                {
                    cubicEndSlop = endSlope;

                    ComputeCubic(cubicList, cubicStartSlop, cubicEndSlop);

                    cubicList.Clear();

                    // last one
                    var profile = profiles[profiles.Count - 1];
                    profile.Status = 2;
                    profile.C0 = 0;
                    profile.C1 = endSlope;
                    profile.C2 = 0;
                    profile.C3 = 0;
                }
            }

            private static void ComputeCubic(List<CamProfile> cubicProfiles, double startSlope = 0,
                double endSlope = 0)
            {
                var count = cubicProfiles.Count;

                var x = new double[count];
                var y = new double[count];

                for (var i = 0; i < count; i++)
                {
                    var profile = cubicProfiles[i];
                    x[i] = profile.X;
                    y[i] = profile.Y;
                }

                var spline = new CubicSpline();
                var resultProfiles = spline.ComputeCamProfiles(x, y, startSlope, endSlope, true);

                if (resultProfiles.Count > 0)
                    for (var i = 0; i < resultProfiles.Count; i++)
                    {
                        cubicProfiles[i].Status = 2;
                        cubicProfiles[i].C0 = resultProfiles[i].C0;
                        cubicProfiles[i].C1 = resultProfiles[i].C1;
                        cubicProfiles[i].C2 = resultProfiles[i].C2;
                        cubicProfiles[i].C3 = resultProfiles[i].C3;
                    }
            }
        }
    }