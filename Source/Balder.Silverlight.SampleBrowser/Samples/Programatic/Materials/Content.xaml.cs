using System.Windows.Controls;
using System.Windows.Media;
using Balder.Core.Assets;
using Balder.Core.Execution;
using Balder.Core.Lighting;
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using Geometry=Balder.Core.Objects.Geometries.Geometry;

namespace Balder.Silverlight.SampleBrowser.Samples.Programatic.Materials
{
	public partial class Content
	{
		private Geometry _container;

		private Mesh _teapot;
		private Box _box;
		private Cylinder _cylinder;
		private Ring _ring;

		private Geometry _selectedNode;
		private Material _selectedMaterial;

		private Material _flatMaterial;
		private Material _gouraudMaterial;
		private Material _texturedMaterial;
		private Material _reflectionMaterial;

		private Core.Imaging.Image _selectedTexture;
		private Core.Imaging.Image _reflectionMapTexture;
		private Core.Imaging.Image _visualStudioTexture;
		private Core.Imaging.Image _balderLogoTexture;

		private bool _initialized = false;


		public Content()
		{
			InitializeComponent();

			Game.LoadContent += Game_LoadContent;
			Game.Initialize += Game_Initialize;
			Game.Update += Game_Update;
		}

		private void Game_LoadContent(Game game)
		{
			_container = new Geometry();
			game.Scene.AddNode(_container);

			_teapot = game.ContentManager.Load<Mesh>("teapot.ASE");
			_container.Children.Add(_teapot);
			_teapot.IsVisible = false;

			_box = new Box {Dimension = new Coordinate(40, 40, 40)};
			_box.Position.Set(0,10,0);
			_container.Children.Add(_box);

			_cylinder = new Cylinder {Segments = 16, Size = 20, TopRadius = 20, BottomRadius = 40};
			_cylinder.Position.Set(0, 10, 0);
			_container.Children.Add(_cylinder);

			_ring = new Ring {Segments = 16, Size = 20, InnerRadius = 20, OuterRadius = 40};
			_ring.Position.Set(0, 10, 0);
			_container.Children.Add(_ring);



			_reflectionMapTexture = LoadTexture("/Balder.Silverlight.SampleBrowser;component/Samples/Programatic/Materials/Assets/ReflectionMap.jpg");
			_visualStudioTexture = LoadTexture("/Balder.Silverlight.SampleBrowser;component/Samples/Programatic/Materials/Assets/VisualStudio.png");
			_balderLogoTexture = LoadTexture("/Balder.Silverlight.SampleBrowser;component/Samples/Programatic/Materials/Assets/BalderLogo.png");

			_flatMaterial = new Material
			{
				Diffuse = Colors.Red,
				Shade = MaterialShade.Flat
			};
			_gouraudMaterial = new Material
			{
				Diffuse = Colors.Red,
				Shade = MaterialShade.Gouraud
			};
			_texturedMaterial = new Material
			{
				DiffuseMap = _reflectionMapTexture,
				Shade = MaterialShade.Flat,
			};
			_reflectionMaterial = new Material
			{
				ReflectionMap = _reflectionMapTexture,
				Shade = MaterialShade.Flat
			};

		}

		private Core.Imaging.Image LoadTexture(string uri)
		{
			// Todo: this is very hacky - refactoring of the asset system will make this not needed!
			var assetLoaderService = KernelContainer.Kernel.Get<IAssetLoaderService>();
			var loader = assetLoaderService.GetLoader<Core.Imaging.Image>(uri);
			var images = loader.Load(uri);
			if (images.Length == 1)
			{
				return images[0];
			}
			return null;
		}


		private void Game_Initialize(Game game)
		{
			game.ContentManager.AssetsRoot = "Samples/Programatic/Materials/Assets";

			var light = new OmniLight
			            	{
			            		Ambient = Colors.Black,
			            		Diffuse = Colors.Black,
			            		Specular = Colors.White,
			            		Position = new Coordinate(0, 400, 0)
			            	};
			game.Scene.AddNode(light);

			game.Camera.Position.Set(0,80,-150);
			game.Camera.Target.Set(0,10,0);


		}

		private void Game_Update(Game game)
		{
			if( !_initialized)
			{
				HandleSelection();
			}
			_container.Rotation.Y += 1;
		}

		private void ObjectTypeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (null != ObjectComboBox)
			{
				HandleSelection();
			}
		}


		private void MaterialTypeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (null != MaterialComboBox)
			{
				HandleMaterialSelection();
			}
		}

		private void TextureChanged(object sender, SelectionChangedEventArgs e)
		{
			if (null != TextureComboBox)
			{
				HandleTextureSelection();
			}
		}


		private void HandleSelection()
		{
			HandleObjectSelection();
			HandleMaterialSelection();
			HandleTextureSelection();
		}

		private void HandleObjectSelection()
		{
			switch (ObjectComboBox.SelectedIndex)
			{
				case 0:
					{
						_selectedNode = _teapot;

						_teapot.IsVisible = true;
						_box.IsVisible = false;
						_cylinder.IsVisible = false;
						_ring.IsVisible = false;
					}
					break;
				case 1:
					{
						_selectedNode = _box;

						_teapot.IsVisible = false;
						_box.IsVisible = true;
						_cylinder.IsVisible = false;
						_ring.IsVisible = false;
					}
					break;
				case 2:
					{
						_selectedNode = _cylinder;

						_teapot.IsVisible = false;
						_box.IsVisible = false;
						_cylinder.IsVisible = true;
						_ring.IsVisible = false;
					}
					break;
				case 3:
					{
						_selectedNode = _ring;

						_teapot.IsVisible = false;
						_box.IsVisible = false;
						_cylinder.IsVisible = false;
						_ring.IsVisible = true;
					}
					break;
			}
		}

		private void HandleMaterialSelection()
		{

			switch (MaterialComboBox.SelectedIndex)
			{
				case 0:
					{
						_selectedMaterial = _flatMaterial;
					}
					break;
				case 1:
					{
						_selectedMaterial = _gouraudMaterial;
					}
					break;
				case 2:
					{
						_selectedMaterial = _texturedMaterial;
					}
					break;
				case 3:
					{
						_selectedMaterial = _reflectionMaterial;
					}
					break;
			}

			_selectedNode.Material = _selectedMaterial;

			HandleTextureSelection();
		}

		private void HandleTextureSelection()
		{
			switch (TextureComboBox.SelectedIndex)
			{
				case 0:
					{
						_selectedTexture = _reflectionMapTexture;
					}
					break;
				case 1:
					{
						_selectedTexture = _visualStudioTexture;
					}
					break;
				case 2:
					{
						_selectedTexture = _balderLogoTexture;
					}
					break;
			}

			if (null != _selectedMaterial.DiffuseMap)
			{
				_selectedMaterial.DiffuseMap = _selectedTexture;
			}
			else if (null != _selectedMaterial.ReflectionMap)
			{
				_selectedMaterial.ReflectionMap = _selectedTexture;
			}
		}
	}
}
