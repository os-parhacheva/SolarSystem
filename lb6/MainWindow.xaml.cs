using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lb6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Background = Brushes.Black;

            Viewport3D viewport = new Viewport3D();
            Content = viewport;

            Model3DGroup lightGroup = new Model3DGroup();         
            lightGroup.Children.Add(new AmbientLight(Color.FromRgb(41, 38, 38)));
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(255,255,255),
                                            new Vector3D(2, -3, -1)));

            ModelVisual3D lightModvis = new ModelVisual3D();
            lightModvis.Content = lightGroup;
            viewport.Children.Add(lightModvis);

            PerspectiveCamera cam = new PerspectiveCamera(new Point3D(4, 0, 20),
                new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 45);

            Trackball trb = new Trackball();
            cam.Transform = trb.Transform;
            trb.EventSource = this;

            viewport.Camera = cam;

            List<GeneratePlanet> solarSystem = new List  <GeneratePlanet> ();
            //Sun
            solarSystem.Add(new GeneratePlanet(radius: 0.2, zAngleRotation: 23.44, typeObject: TypeObject.Star));
            //Mercury
            solarSystem.Add(new GeneratePlanet(radius: 0.2, rotationTime: 57, zAngleRotation: 7, orbitRadius: 1, sunRotationTime: 70) { InCenter = solarSystem[0].InCenter});
            //Venus
            solarSystem.Add(new GeneratePlanet(radius: 0.2, rotationTime: 243, zAngleRotation: 3, orbitRadius: 2, sunRotationTime: 200) { InCenter = solarSystem[0].InCenter});

            //Earth
            solarSystem.Add(new GeneratePlanet(radius: 0.2, rotationTime: 1, zAngleRotation: 23.44, orbitRadius: 4, sunRotationTime: 365) { InCenter = solarSystem[0].InCenter});
            //Moon
            solarSystem.Add(new GeneratePlanet(radius: 0.1, rotationTime: 1, orbitRadius: 0.8, sunRotationTime: 365,  orbitRotationTime: 1, typeObject: TypeObject.Satellite) { InCenter = solarSystem[3].InCenter });
            
            //Mars
            solarSystem.Add(new GeneratePlanet(radius: 0.2, rotationTime: 1, zAngleRotation: 25, orbitRadius: 7, sunRotationTime: 500) { InCenter = solarSystem[0].InCenter });
            //Phobos
            solarSystem.Add(new GeneratePlanet(radius: 0.1, rotationTime: 0.8, orbitRadius: 0.6, sunRotationTime: 500, orbitRotationTime: 3,  typeObject: TypeObject.Satellite) { InCenter = solarSystem[5].InCenter });
            //Deimos
            solarSystem.Add(new GeneratePlanet(radius: 0.1, rotationTime: 2, orbitRadius: 1, sunRotationTime: 500, orbitRotationTime: 0.6, typeObject: TypeObject.Satellite) { InCenter= solarSystem[5].InCenter });

     
            solarSystem.ForEach(celestialObject => viewport.Children.Add(celestialObject.Model3D));
        }
    }
}
