/// Credit Farfarer
/// Sourced from - https://gist.github.com/Farfarer/a765cd07920d48a8713a0c1924db6d70
/// Updated for UI / 2D - SimonDarksideJ

using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [System.Serializable]
    public class CableCurve
    {
        [SerializeField]
        Vector2 m_start;
        [SerializeField]
        Vector2 m_end;
        [SerializeField]
        float m_slack;
        [SerializeField]
        int m_steps;
        [SerializeField]
        bool m_regen;

        static Vector2[] emptyCurve = new Vector2[] { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };
        [SerializeField]
        Vector2[] points;

        public bool regenPoints
        {
            get { return m_regen; }
            set
            {
                m_regen = value;
            }
        }

        public Vector2 start
        {
            get { return m_start; }
            set
            {
                if (value != m_start)
                    m_regen = true;
                m_start = value;
            }
        }

        public Vector2 end
        {
            get { return m_end; }
            set
            {
                if (value != m_end)
                    m_regen = true;
                m_end = value;
            }
        }
        public float slack
        {
            get { return m_slack; }
            set
            {
                if (value != m_slack)
                    m_regen = true;
                m_slack = Mathf.Max(0.0f, value);
            }
        }
        public int steps
        {
            get { return m_steps; }
            set
            {
                if (value != m_steps)
                    m_regen = true;
                m_steps = Mathf.Max(2, value);
            }
        }

        public Vector2 midPoint
        {
            get
            {
                Vector2 mid = Vector2.zero;
                if (m_steps == 2)
                {
                    return (points[0] + points[1]) * 0.5f;
                }
                else if (m_steps > 2)
                {
                    int m = m_steps / 2;
                    if ((m_steps % 2) == 0)
                    {
                        mid = (points[m] + points[m + 1]) * 0.5f;
                    }
                    else
                    {
                        mid = points[m];
                    }
                }
                return mid;
            }
        }

        public CableCurve()
        {
            points = emptyCurve;
            m_start = Vector2.up;
            m_end = Vector2.up + Vector2.right;
            m_slack = 0.5f;
            m_steps = 20;
            m_regen = true;
        }

        public CableCurve(Vector2[] inputPoints)
        {
            points = inputPoints;
            m_start = inputPoints[0];
            m_end = inputPoints[1];
            m_slack = 0.5f;
            m_steps = 20;
            m_regen = true;
        }

        public CableCurve(List<Vector2> inputPoints)
        {
            points = inputPoints.ToArray();
            m_start = inputPoints[0];
            m_end = inputPoints[1];
            m_slack = 0.5f;
            m_steps = 20;
            m_regen = true;
        }

        public CableCurve(CableCurve v)
        {
            points = v.Points();
            m_start = v.start;
            m_end = v.end;
            m_slack = v.slack;
            m_steps = v.steps;
            m_regen = v.regenPoints;
        }

        public Vector2[] Points()
        {
            if (!m_regen)
                return points;

            if (m_steps < 2)
                return emptyCurve;

            float lineDist = Vector2.Distance(m_end, m_start);
            float lineDistH = Vector2.Distance(new Vector2(m_end.x, m_start.y), m_start);
            float l = lineDist + Mathf.Max(0.0001f, m_slack);
            float r = 0.0f;
            float s = m_start.y;
            float u = lineDistH;
            float v = end.y;

            if ((u - r) == 0.0f)
                return emptyCurve;

            float ztarget = Mathf.Sqrt(Mathf.Pow(l, 2.0f) - Mathf.Pow(v - s, 2.0f)) / (u - r);

            int loops = 30;
            int iterationCount = 0;
            int maxIterations = loops * 10; // For safety.
            bool found = false;

            float z = 0.0f;
            float ztest = 0.0f;
            float zstep = 100.0f;
            float ztesttarget = 0.0f;
            for (int i = 0; i < loops; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    iterationCount++;
                    ztest = z + zstep;
                    ztesttarget = (float)Math.Sinh(ztest) / ztest;

                    if (float.IsInfinity(ztesttarget))
                        continue;

                    if (ztesttarget == ztarget)
                    {
                        found = true;
                        z = ztest;
                        break;
                    }
                    else if (ztesttarget > ztarget)
                    {
                        break;
                    }
                    else
                    {
                        z = ztest;
                    }

                    if (iterationCount > maxIterations)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    break;

                zstep *= 0.1f;
            }

            float a = (u - r) / 2.0f / z;
            float p = (r + u - a * Mathf.Log((l + v - s) / (l - v + s))) / 2.0f;
            float q = (v + s - l * (float)Math.Cosh(z) / (float)Math.Sinh(z)) / 2.0f;

            points = new Vector2[m_steps];
            float stepsf = m_steps - 1;
            float stepf;
            for (int i = 0; i < m_steps; i++)
            {
                stepf = i / stepsf;
                Vector2 pos = Vector2.zero;
                pos.x = Mathf.Lerp(start.x, end.x, stepf);
                pos.y = a * (float)Math.Cosh(((stepf * lineDistH) - p) / a) + q;
                points[i] = pos;
            }

            m_regen = false;
            return points;
        }
    }
}