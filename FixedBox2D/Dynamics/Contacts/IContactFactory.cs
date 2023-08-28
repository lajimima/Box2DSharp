namespace FixedBox2D.Dynamics.Contacts
{
    internal interface IContactFactory
    {
        Contact Create(Fixture fixtureA, int indexA, Fixture fixtureB, int indexB);

        void Destroy(Contact contact);
    }
}