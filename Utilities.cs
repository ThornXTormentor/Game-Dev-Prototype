using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utilities
{
	public Utilities()
	{

	}

    public enum WeaponType
    {
        //Set enums for weapons that are switched out when selected / gained
        none,
        blaster,
        spread,
        phaser,
        missile,
        laser,
        shield
    }

    [System.Serializable]
    public class WeaponDefinition
    {
        //Defines the base of the weapon
        public WeaponType type = WeaponType.none;
        public string letter;
        public Color color = Color.white;
        public GameObject projectilePrefab;
        public Color projectileColor = Color.white;
        public float damageOnHit = 0;
        public float continuousDamage = 0;
        public float delayBetweenShots = 0;
        public float velocity = 20;
    }

    //Set up of a weapon class
    public class Weapon : MonoBehaviour
    {
        static public Transform PROJECTILE_ANCHOR;

        [Header("Set Dynamically")]
        [SerializeField]
        private WeaponType _type = WeaponType.none;
        public WeaponDefinition def;
        public GameObject collar;
        public float lastShotTime;
        private Renderer collarRend;

        // Start is called before the first frame update
        void Start()
        {
            collar = transform.Find("Collar").gameObject;
            collarRend = collar.GetComponent<Renderer>();

            SetType(_type);

            if (PROJECTILE_ANCHOR == null)
            {
                GameObject go = new GameObject("_ProjectileAnchor");
                PROJECTILE_ANCHOR = go.transform;
            }

            GameObject rootGO = transform.root.gameObject;
            if (rootGO.GetComponent<Hero>() != null)
            {
                rootGO.GetComponent<Hero>().fireDelegate += Fire;
            }
        }

        public WeaponType type
        {
            get { return (_type); }
            set { SetType(value); }
        }

        public void SetType(WeaponType wt)
        {
            _type = wt;
            if (type == WeaponType.none)
            {
                this.gameObject.SetActive(false);
                return;
            }
            else
            {
                this.gameObject.SetActive(true);
            }
            def = Main.GetWeaponDefinition(_type);
            collarRend.material.color = def.color;
            lastShotTime = 0;
        }

        public void Fire()
        {
            if (!gameObject.activeInHierarchy) return;
            if (Time.time - lastShotTime < def.delayBetweenShots)
            {
                return;
            }
            Projectile p;
            Vector3 vel = Vector3.up * def.velocity;
            if (transform.up.y < 0)
            {
                vel.y = -vel.y;
            }
            switch (type)
            {
                case WeaponType.blaster:
                    p = MakeProjectile();
                    p.rigid.velocity = vel;
                    break;
                case WeaponType.spread:
                    p = MakeProjectile();
                    p.rigid.velocity = vel;
                    p = MakeProjectile();
                    p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                    p.rigid.velocity = p.transform.rotation * vel;
                    p = MakeProjectile();
                    p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                    p.rigid.velocity = p.transform.rotation * vel;
                    break;
            }
        }

        public Projectile MakeProjectile()
        {
            GameObject go = Instantiate<GameObject>(def.projectilePrefab);
            if (transform.parent.gameObject.tag == "Hero")
            {
                go.tag = "ProjectileHero";
                go.layer = LayerMask.NameToLayer("ProjectileHero");
            }
            else
            {
                go.tag = "ProjectileEnemy";
                go.layer = LayerMask.NameToLayer("ProjectileEnemy");
            }
            go.transform.position = collar.transform.position;
            go.transform.SetParent(PROJECTILE_ANCHOR, true);
            Projectile p = go.GetComponent<Projectile>();
            p.type = type;
            lastShotTime = Time.time;
            return (p);
        }
    }

    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }

        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

    //checks bounds first, then spawns enemy within bounds on top of screen
    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    //Checks strings if they are numbers
    public static string ProcessText(string textIn)
    {
        string textOut = "";

        if (Int32.TryParse(textIn, out int value))
        {
            textOut = "Integer";
        }
        else if (Double.TryParse(textIn, out double fvalue))
        {
            textOut = "Double";
        }
        else
        {
            textOut = "String";
        }

        return textOut;
    }

    //Checks on what powerup is absorbed
    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pu.type == weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    //Enemy movement sample
    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }
    //Find destructable off of string
    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return null;
    }
    //find destructable off of gameobject
    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return null;
    }

    //true if object/string is destroyed
    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt)
    {
        if (prt == null)
        {
            return (true);
        }

        return (prt.health <= 0);
    }

    //Show spots where multi part entity is being hit locally
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;

    }

    //checks when being hit by projectile
    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }

                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false);
                }

                bool allDestroyed = true;
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }
}
