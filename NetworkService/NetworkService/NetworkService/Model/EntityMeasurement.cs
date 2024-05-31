using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class EntityMeasurement
    {
        int entityIndex;
        float measurementValue;
        public EntityMeasurement(int entityIndex, float measurementValue)
        {
            this.entityIndex = entityIndex;
            this.measurementValue = measurementValue;
        }
        public int EntityIndex
        {
            get { return entityIndex; }
            set
            {
                entityIndex = value;
            }
        }
        public float MeasurementValue
        {
            get { return (float)Math.Round(measurementValue, 2); }
            set
            {
                measurementValue = value;
            }
        }
    }
}
