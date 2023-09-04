using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaiyanLib.Abstracts
{
    public class Gradient
    {
        private readonly List<Tuple<float, Color>> gradientStops;

        public Gradient()
        {
            gradientStops = new List<Tuple<float, Color>>();
        }

        /// <param name="color">Starting color of this gradient.</param>
        public Gradient(Color color)
        {
            gradientStops = new List<Tuple<float, Color>>();
            gradientStops.Add(new Tuple<float, Color>(0f, color));
        }
        public Gradient(Color startColor, params (float percent, Color color)[] subsequentColors)
        {
            gradientStops = new List<Tuple<float, Color>>() {
                new Tuple<float, Color>(0, startColor)
            };
            gradientStops.AddRange(from color_value_pair in subsequentColors select new Tuple<float, Color>(color_value_pair.percent, color_value_pair.color));
        }

        /// <summary>
        /// Adds a new color point to this gradient at a specified percent point in its length.
        /// </summary>
        /// <param name="percent">0 to 1 value on where this new color kicks in.</param>
        /// <param name="color">Color value.</param>
        public Gradient AddStop(float percent, Color color)
        {
            gradientStops.Add(new Tuple<float, Color>(percent, color));
            return this;
        }

        public Color GetColor(double percent)
        {
            if (percent < 0 || percent > 1)
                throw new ArgumentOutOfRangeException(nameof(percent), "Percent must be between 0 and 1");

            if (gradientStops.Count == 0)
                return Color.Black;

            if (gradientStops.Count == 1)
                return gradientStops[0].Item2;

            // Find the two nearest gradient stops
            Tuple<float, Color> stop1 = null!;
            Tuple<float, Color> stop2 = null!;
            for (int i = 0; i < gradientStops.Count; i++)
            {
                if (gradientStops[i].Item1 <= percent)
                {
                    stop1 = gradientStops[i];
                }
                else
                {
                    stop2 = gradientStops[i];
                    break;
                }
            }

            // If there is no stop2, then the percent is greater than all of the gradient stops, so return the last stop's color
            if (stop2 == null)
                return stop1.Item2;

            // Find the percent distance between the two nearest stops
            double percentDistance = stop2.Item1 - stop1.Item1;
            double percentThroughStops = (percent - stop1.Item1) / percentDistance;

            // Find the R, G, and B values of the color between the two stops
            double r = stop1.Item2.R + (stop2.Item2.R - stop1.Item2.R) * percentThroughStops;
            double g = stop1.Item2.G + (stop2.Item2.G - stop1.Item2.G) * percentThroughStops;
            double b = stop1.Item2.B + (stop2.Item2.B - stop1.Item2.B) * percentThroughStops;

            return new Color((int)r, (int)g, (int)b);
        }
    }
}
