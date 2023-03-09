namespace ICSStudio.Database.Table.Motion
{
    public class PMRotaryMotor
    {
        // ReSharper disable once InconsistentNaming
        public int MotorID { get; set; }
        public int PolesPerRev { get; set; }
        public float Inertia { get; set; }
        public float Resistance { get; set; }
        public float Inductance { get; set; }
        public float FluxSaturation1 { get; set; }
        public float FluxSaturation2 { get; set; }
        public float FluxSaturation3 { get; set; }
        public float FluxSaturation4 { get; set; }
        public float FluxSaturation5 { get; set; }
        public float FluxSaturation6 { get; set; }
        public float FluxSaturation7 { get; set; }
        public float FluxSaturation8 { get; set; }
        public float RatedTorque { get; set; }
        public float TorqueConstant { get; set; }
        public float VoltageConstant { get; set; }
        public float CommutationOffset { get; set; }
        public float RatedVelocity { get; set; }
        public float MaxVelocity { get; set; }
        public int DampCoefficient { get; set; }
        public bool FactoryAligned { get; set; }
        public float Acceleration { get; set; }
    }
}
