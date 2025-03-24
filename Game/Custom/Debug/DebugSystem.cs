using System;
using Game.Custom.Components;
using Game.Custom.Input;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGameGum;
using MonoGameGum.Forms;
using MonoGameGum.Forms.Controls;

namespace Game.Custom.Debug;


public class DebugSystem() : EntitySystem(Aspect.All(typeof(Transform2))), IUpdateSystem, IDrawSystem
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<Texture2D> _textureMapper;
    private ComponentMapper<SpriteComponent> _spriteMapper;
    private ComponentMapper<AnimatedSprite> _animatedMapper;

    private Entity _selectedEntity;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _textureMapper = mapperService.GetMapper<Texture2D>();
        _spriteMapper = mapperService.GetMapper<SpriteComponent>();
        _animatedMapper = mapperService.GetMapper<AnimatedSprite>();

        var button = Global.Game.Root.GetFrameworkElementByName<Button>("ButtonStandardInstance");
        button.Click += (_, _) => { _selectedEntity?.Destroy(); _selectedEntity = null; };
    }

    public void Update(GameTime gameTime)
    {
        if (!InputManager.MouseClicked)
            return;

        if (GumService.Default.Cursor.WindowOver is GraphicalUiElement element) // Ignore clicks that are on UI elements
        {
            Console.WriteLine(element.Name);
            return;
        }

        var mousePosition = InputManager.MouseRectangle.Location.ToVector2();
        var worldPosition = Global.Camera.ScreenToWorld(mousePosition);

        foreach (var entity in ActiveEntities)
        {
            var transfrom = _transformMapper.Get(entity);
            var relBBox = GetRelativeBBox(entity);
            var bbox = new RectangleF(relBBox.Position + transfrom.Position - relBBox.Size / 2, relBBox.Size);

            if (bbox.Contains(worldPosition))
            {
                _selectedEntity = GetEntity(entity);
                return;
            }
        }

        _selectedEntity = null;
    }

    private RectangleF GetRelativeBBox(int entity)
    {
        if (_textureMapper.Has(entity)) return _textureMapper.Get(entity).Bounds;
        if (_spriteMapper.Has(entity)) return _spriteMapper.Get(entity).Texture.Bounds;
        if (_animatedMapper.Has(entity)) return (_animatedMapper.Get(entity) is AnimatedSprite t) ? t.GetBoundingRectangle(new(t.GetBoundingRectangle(new()).Size/2)) : default; // very cursed
        return new(0, 0, 10, 10);
    }

    public void Draw(GameTime gameTime)
    {
        if (_selectedEntity is null)
            return;

        var relBBox = GetRelativeBBox(_selectedEntity.Id);
        Global.SpriteBatch.DrawRectangle(relBBox.Position + _selectedEntity.Get<Transform2>().Position - relBBox.Size / 2, relBBox.Size, Color.DeepPink, 2f);
    }
}
