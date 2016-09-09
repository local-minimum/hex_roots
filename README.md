# hex_roots (dev name)

This project is inspired by a game I played during the Ludum 36 rating period 
[Aqueducts of Solomon by Lone Spelunker](http://ludumdare.com/compo/ludum-dare-36/?action=preview&uid=112022).

The goal is to produce a mobile-friendly solitair hex-tiled game about resource management 
but initial level/card set will be about being the root of a plant.

You will have to balance the resource economy to be able to grow and store enough nutrients to become perennial.

* Each tile will have a **function**, a **cost of production**, a **victory score**.
* To survive the winter, you must obtain a certain amount of victory score before the end of the growth season.
* The player has a hand of cards (3 - 5 depending on difficulty?) to choose from. At the end of each turn, the hand is replentished until the deck runs out.
* Cards with positive **victory score** can be destroyed in exchange for **resources**, cards with zero **victory score** 
can be destroyed but without resource gain.
* Cards with negative **victory score** can not be destroyed and must be kept in hand or be played out on the board where they will both decrease the 
general viability of the root as well as affect the neighbouring tiles.
* Playing an adversive card allows the player to immidiately draw a new card to the hand.
* **resources** are used to construct tiles from cards on hand. Each *meristem tile* may produce on new tile per round.
* There are four **resources**: *water*, *sugar*, *nitrogen* and *minerals*. 
* The game starts with the special card (never available in the deck) *root base* at the top center and a *meristem tile* directly below it.
* The *root base* is a trading post between root and the shoot. This is the only place to obtain *sugar*. There are different exchange rates depending on what **resources** you offer. Some exchanges have prolonged effect over several rounts.
* A *meristem* may only construct new tiles such that they connect "laterally" to itself or connects to the most proximate root.
* A *meristem* when producing a *root*, places the root tile at the position previously occupied by the *meristem* and then places
the *meristem* at a neighbouring position.
* *roots* transport **resources**.
* New *meristems* allow for branching root system. Victory Points: 0
* *root hairs* produce water or minerals. Victory Points: 1
* *nodules* uses *sugar* in exchange for *nitrogen*. Victory Points: 1
* *storage roots* gain victory points by amount of *sugar* in them.
