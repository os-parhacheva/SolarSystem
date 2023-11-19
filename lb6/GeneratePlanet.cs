using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace lb6
{

    public enum TypeObject
    {
        Planet = 0,
        Star = 1,
        Satellite= 2
    }
    public class GeneratePlanet
    {
        double radius;
        double rotationTime;
        double zAngleRotation;
        bool reverseRotation;

        double orbitRadius;
        double orbitRotationTime;
        bool reverseOrbitRotation;

        double sunRotationTime;

        Point3D outCenter;
        Point3D inCenter;

        TypeObject typeObject;
        public Point3D InCenter
        {
            get
            {
                return inCenter;
            }
            set
            {
                inCenter = value +  new Vector3D(orbitRadius, 0, 0); //Положение планеты
                outCenter = value;   //Положение основной планеты (главной)
            }
        }

        //необходимые параметры
        public GeneratePlanet(  double radius = 0.1,
                                double rotationTime = 0,
                                double zAngleRotation = 0,
                                bool reverseRotation = false,
                                double orbitRadius = 0,
                                double orbitRotationTime = 0,
                                bool reverseOrbitRotation = false,                             
                                double sunRotationTime=0,
                                TypeObject typeObject = 0
                               )
        {
            this.radius = radius;
            this.rotationTime = rotationTime;
            this.zAngleRotation = zAngleRotation;
            this.reverseRotation = reverseRotation;
            this.orbitRadius = orbitRadius;
            this.orbitRotationTime = orbitRotationTime;
            this.reverseOrbitRotation = reverseOrbitRotation;
            this.typeObject = typeObject;            
            this.sunRotationTime = sunRotationTime;
        }
        public static MeshGeometry3D Sphere(Point3D center, double radius, int slices, int stacks)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int stack = 0; stack <= stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / stacks;
                double y = radius * Math.Sin(phi);
                double scale = -radius * Math.Cos(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    double theta = slice * 2 * Math.PI / slices;
                    double x = scale * Math.Sin(theta);
                    double z = scale * Math.Cos(theta);

                    Vector3D normal = new Vector3D(x, y, z);
                    mesh.Normals.Add(normal);
                    mesh.Positions.Add(normal + center);
                    mesh.TextureCoordinates.Add(new Point((double)slice / slices, (double)stack / stacks));

                    int n = slices + 1;

                    if (stack != 0)
                    {
                        mesh.TriangleIndices.Add((stack + 0) * n + slice);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice);
                        mesh.TriangleIndices.Add((stack + 0) * n + slice + 1);
                    }
                    if (stack != stacks - 1)
                    {
                        mesh.TriangleIndices.Add((stack + 0) * n + slice + 1);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice + 1);
                    }
                }
            }
            mesh.Freeze();
            return mesh;
        }

        private RotateTransform3D GetRotation(Point3D rotationCenter, double time, bool reverse = false)
        {
            AxisAngleRotation3D rotation;
            DoubleAnimation animation;

            animation = new DoubleAnimation();
            animation.RepeatBehavior = RepeatBehavior.Forever;
            // Установка оси вращения 
            rotation = new AxisAngleRotation3D();
            rotation.Axis = new Vector3D(0, 1, 0);

            animation.From = reverse ? 360 : 0;
            animation.To = reverse ? 0 : 360;
            animation.Duration = TimeSpan.FromSeconds(time);

            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);

            return new RotateTransform3D(rotation, rotationCenter);
        }

        public ModelVisual3D Model3D
        {
            get
            {
              
                var mesh = Sphere(InCenter, radius, 20, 20);
                mesh.Freeze();

                var geomod = new GeometryModel3D();
                geomod.Geometry = mesh;
                //цвет
                var materials = new MaterialGroup();
                materials.Children.Add(new DiffuseMaterial(Brushes.Blue));
                if (typeObject== TypeObject.Star) materials.Children.Add(new EmissiveMaterial(Brushes.Yellow));

                geomod.Material = materials;

                ModelVisual3D modvis = new ModelVisual3D();
                modvis.Content = geomod;

                var transforms = new Transform3DGroup();
               
                //поворот
                transforms.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), zAngleRotation), InCenter));

                //вокруг своей оси
                if (this.rotationTime != 0) { transforms.Children.Add(GetRotation(InCenter, this.rotationTime, reverseRotation)); }
                //вокруг планеты
                if (typeObject == TypeObject.Satellite) transforms.Children.Add(GetRotation(outCenter, orbitRotationTime, !reverseOrbitRotation));
                
                //вокруг солнца
                if (typeObject != TypeObject.Star) transforms.Children.Add(GetRotation(new Point3D(0,0,0), sunRotationTime, !reverseOrbitRotation));

                modvis.Transform = transforms;

                return modvis;
            }
        }



    }
}
