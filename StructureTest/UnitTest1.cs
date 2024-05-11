using FEA2D.Elements;
using FEA2D.Materials;
using FEA2D.CrossSections;
using FEA2D.Loads;
using FEA2D.Structures;

namespace StructureTest
{
    public class Tests
    {
        Structure structure;
        GenericIsotropicMaterial material;
        IFrame2DSection section;
        LoadCase loadCase;
        [SetUp]
        public void Setup()
        {
            // units are kN, m
            structure = new Structure();
            material = new GenericIsotropicMaterial() { E = 2E11, U = 0.2, Label = "Steel", Alpha = 0, Gama = 20, MaterialType = MaterialType.Steel };
            section = new Generic2DSection(0.05, 0.05, 0.05, 0.0001, 0.0001, 0.0001 * 2, 1, 1, material);
            loadCase = new LoadCase("live", LoadCaseType.Live);
            structure.LoadCasesToRun.Add(loadCase);
            structure.LinearMesher.NumberSegements = 20;
        }

        [Test]
        public void Test1()
        {
            bool abc = Convert.ToBoolean(0);
            structure.fileName = "Structure1";
            Node2D n1 = new Node2D(0, 0, "n1");
            Node2D n2 = new Node2D(0, 3, "n2");
            structure.AddNode(n1, n2);
            n1.Support = new NodalSupport(true, true, true); //fully restrained


            FrameElement2D e1 = new FrameElement2D(n1, n2, "e1") { CrossSection = section };
            structure.AddElement(new[] { e1 });

            n2.NodalLoads.Add(new NodalLoad(-10, 0, 0, LoadDirection.Global, loadCase));

            structure.Solve();
            var R1 = structure.Results.GetSupportReaction(n1, loadCase);

            Assert.AreEqual((R1.Fx - 10.000) < 0.1, true);
            Assert.AreEqual((R1.Fy - 0) < 0.1, true);
            Assert.AreEqual((R1.Mz + 30.000) < 0.1, true);
        }
    }
}