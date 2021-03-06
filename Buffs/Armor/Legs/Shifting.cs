﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Legs
{
	public class Shifting : LegBuff
	{
		public override string Name => "Shifting";
		public override string Tooltip => "Don't lose control!";
		public override string Texture => $"Terraria/buff_{BuffID.Swiftness}";
		private int dashTimer;

		public override void UpdateEquips(VPlayer player, ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			if (player.player.dash > 0)
			{
				player.player.dash = -1;
			}

			if (player.player.dashTime > 0)
			{
				player.player.dashTime--;
			}

			if (player.player.dashTime < 0)
			{
				player.player.dashTime++;
			}

			if (dashTimer > 0)
			{
				dashTimer--;
			}

			if (dashTimer < 0)
			{
				dashTimer++;
			}

			if (player.player.dashDelay > 0)
			{
				player.player.dashDelay--;
				return;
			}

			bool flag = false;
			int lor = 0;
			int uod = 0;

			if (player.player.controlLeft && player.player.releaseLeft)
			{
				if (player.player.dashTime < 0)
				{
					lor = -1;
					flag = true;
					player.player.dashTime = 0;
				}
				else
				{
					player.player.dashTime = -15;
				}
			}
			else if (player.player.controlRight && player.player.releaseRight)
			{
				if (player.player.dashTime > 0)
				{
					lor = 1;
					flag = true;
					player.player.dashTime = 0;
				}
				else
				{
					player.player.dashTime = 15;
				}
			}

			if (player.player.controlUp && player.player.releaseUp && player.player.grapCount == 0)
			{
				if (dashTimer > 0)
				{
					uod = -1;
					flag = true;
					dashTimer = 0;
				}
				else
				{
					dashTimer = 15;
				}
			}
			else if (player.player.controlDown && player.player.releaseDown)
			{
				if (dashTimer < 0)
				{
					uod = 1;
					flag = true;
					dashTimer = 0;
				}
				else
				{
					dashTimer = -15;
				}
			}

			if (flag)
			{
				Vector2 direction = new Vector2(240 * lor, 240 * uod);
				Vector2 position = player.player.position;
				int width = player.player.width;
				int height = player.player.height;
				int i = 0;
				while (i <= 240 && !(direction == default))
				{
					for (int val = -5; val < 6; val++)
					{
						Vector2 destination = position + new Vector2(direction.X, direction.Y - val * 16);
						if (!Collision.SolidCollision(destination, width, height) && Collision.SolidCollision(destination + new Vector2(0f, 16f), width, height))
						{
							direction = new Vector2(direction.X, direction.Y - val * 16);
							break;
						}
					}
					if (!Collision.SolidCollision(position + direction, width, height))
					{
						break;
					}
					if (direction.X > 0f)
					{
						direction.X -= 1f;
					}
					if (direction.X < 0f)
					{
						direction.X += 1f;
					}
					if (direction.Y > 0f)
					{
						direction.Y -= 1f;
					}
					if (direction.Y < 0f)
					{
						direction.Y += 1f;
					}
					i++;
				}
				float ox = position.X;
				float oy = position.Y;
				player.player.grapCount = 0;
				player.player.BetterTeleport(player.player.position + direction, 0);

				if (Main.netMode != NetmodeID.Server)
				{
					if (uod == -1 && lor == 0 && direction.X == 0f)
					{
						player.player.position.X = ox;
					}

					if (uod == 0 && lor != 0 && direction.Y == 0f)
					{
						player.player.position.Y = oy;
					}

					player.player.dashDelay = 120;
				}
			}
		}
	}
}