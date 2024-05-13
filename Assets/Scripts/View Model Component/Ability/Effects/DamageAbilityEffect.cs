using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageAbilityEffect : BaseAbilityEffect 
{
	#region Public

	public GameObject damagePrefab;
	public AudioSource effectsAudioSource;
    public AudioClip hitSound;
    public AudioClip missSound;
    private AudioSource audioSource;

    void Awake() 
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            // Log an error if no AudioSource is found
            Debug.LogError("No AudioSource component found on " + gameObject.name);
        }
    }

	
    
    private void DisplayDamage(int damage, Transform unitTransform)
    {
        // Ensure damagePrefab is assigned
        if (damagePrefab == null)
        {
            Debug.LogError("DamagePrefab is not assigned on " + gameObject.name);
            return;
        }
        
        GameObject damageText = Instantiate(damagePrefab, unitTransform.position, Quaternion.identity, unitTransform);
        damageText.GetComponent<DamageTextScript>().DisplayDamage(damage);
    }

	public override int Predict (Tile target)
	{
		Unit attacker = GetComponentInParent<Unit>();
		Unit defender = target.content.GetComponent<Unit>();

		// Get the attackers base attack stat considering
		// mission items, support check, status check, and equipment, etc
		int attack = GetStat(attacker, defender, GetAttackNotification, 0);

		// Get the targets base defense stat considering
		// mission items, support check, status check, and equipment, etc
		int defense = GetStat(attacker, defender, GetDefenseNotification, 0);

		// Calculate base damage
		int damage = attack - (defense / 2);
		damage = Mathf.Max(damage, 1);

		// Get the abilities power stat considering possible variations
		int power = GetStat(attacker, defender, GetPowerNotification, 0);

		// Apply power bonus
		damage = power * damage / 100;
		damage = Mathf.Max(damage, 1);

		// Tweak the damage based on a variety of other checks like
		// Elemental damage, Critical Hits, Damage multipliers, etc.
		damage = GetStat(attacker, defender, TweakDamageNotification, damage);

		// Clamp the damage to a range
		damage = Mathf.Clamp(damage, minDamage, maxDamage);
		return -damage;
	}
	
	protected override int OnApply (Tile target)
	{
		Unit defender = target.content.GetComponent<Unit>();

		// Start with the predicted damage value
		int value = Predict(target);

		// Add some random variance
		value = Mathf.FloorToInt(value * UnityEngine.Random.Range(0.9f, 1.1f));

		// Clamp the damage to a range
		value = Mathf.Clamp(value, minDamage, maxDamage);

		// Apply the damage to the target
		Stats s = defender.GetComponent<Stats>();
		s[StatTypes.HP] += value;

	DisplayDamage(value, defender.transform);

         if (value > 0)
        {
            StartCoroutine(PlaySound(hitSound));
        }
        else
        {
            StartCoroutine(PlaySound(missSound));
        }

		

		return value;
	}

	 private IEnumerator PlaySound(AudioClip clip)
    {
        if (effectsAudioSource != null && clip != null)
        {
            effectsAudioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(1.0f);  // Wait for 1 second
            effectsAudioSource.Stop();  // Stop the AudioSource
        }
    }

	#endregion
}