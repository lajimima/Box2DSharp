using System.Diagnostics;
using System.Runtime.CompilerServices;
using FixedBox2D.Collision;
using FixedBox2D.Collision.Collider;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Contacts
{
    public class ChainAndPolygonContact : Contact
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Evaluate(ref Manifold manifold, in Transform xfA, Transform xfB)
        {
            var chain = (ChainShape)FixtureA.Shape;

            chain.GetChildEdge(out var edge, ChildIndexA);
            CollisionUtils.CollideEdgeAndPolygon(ref manifold, edge, xfA, (PolygonShape)FixtureB.Shape, xfB);
        }
    }

    internal class ChainAndPolygonContactFactory : IContactFactory
    {
        private readonly ContactPool<ChainAndPolygonContact> _pool = new ContactPool<ChainAndPolygonContact>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Contact Create(Fixture fixtureA, int indexA, Fixture fixtureB, int indexB)
        {
            Debug.Assert(fixtureA.ShapeType == ShapeType.Chain);
            Debug.Assert(fixtureB.ShapeType == ShapeType.Polygon);
            var contact = _pool.Get();
            contact.Initialize(fixtureA, indexA, fixtureB, indexB);
            return contact;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(Contact contact)
        {
            _pool.Return((ChainAndPolygonContact)contact);
        }
    }
}