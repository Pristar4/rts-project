#region Info
// -----------------------------------------------------------------------
// V_Skeleton_Anim.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace V_AnimationSystem
{

	/*
	 * Contains the Frames for an animation for a Body Part
	 * */
	public class V_Skeleton_Anim
	{
		public BodyPart bodyPart;
		private readonly bool canLoop = true;

		private V_Skeleton_Frame currentAnimFrame;

		private int currentFrame;

		public bool defaultHasVariableSortingOrder = false;
		private float frameRate;
		private float frameRateOriginal;

		public V_Skeleton_Frame[] frames;
		private int framesLength;

		private float frameTimer;
		private bool[] hasAnimFrameTrigger;
		public bool looped;
		public Action<V_Skeleton_Anim> onAnimComplete;

		public V_UnitSkeleton.OnAnimTrigger onAnimTrigger;
		private bool useUnscaledTime;

		public V_Skeleton_Anim(V_Skeleton_Frame[] _frames, BodyPart _bodyPart,
		                       float _frameRate)
		{
			SetFrames(_frames);
			currentAnimFrame = frames[0];
			bodyPart = _bodyPart;
			frameRate = _frameRate;
			frameRateOriginal = frameRate;
		}

		public void Update(float deltaTime)
		{
			frameTimer = frameTimer - deltaTime;
			while (frameTimer < 0 && (canLoop || !looped))
			{
				frameTimer = frameTimer + frameRate;
				currentFrame = currentFrame + 1;
				if (currentFrame >= framesLength)
				{
					currentFrame = 0;
					looped = true;
				}

				currentAnimFrame = frames[currentFrame];
				if (hasAnimFrameTrigger[currentFrame])
					if (onAnimTrigger != null)
						onAnimTrigger(currentAnimFrame.trigger);

				if (currentFrame >= framesLength - 1)
				{
					// Last frame
					if (!canLoop)
					{
						looped = true;
						if (onAnimComplete != null) onAnimComplete(this);
						break;
					}
					if (onAnimComplete != null) onAnimComplete(this);
				}
			}
		}

		public void UpdateNextFrame()
		{
			Update(frameRate);
		}

		public V_Skeleton_Frame GetFirstFrame()
		{
			return frames[0];
		}

		public V_Skeleton_Frame GetCurrentAnimFrame()
		{
			return currentAnimFrame;
		}

		public V_Skeleton_Frame[] GetFrames()
		{
			return frames;
		}

		private void SetFrames(V_Skeleton_Frame[] frames)
		{
			this.frames = frames;
			framesLength = frames.Length;
			hasAnimFrameTrigger = new bool[framesLength];
			for (var i = 0; i < frames.Length; i++)
				hasAnimFrameTrigger[i]
						= !string.IsNullOrEmpty(frames[i].trigger);
		}
		public List<string> GetTriggerList()
		{
			var ret = new List<string>();
			foreach (var frame in frames)
				if (!string.IsNullOrEmpty(frame.trigger)) // Has trigger
					ret.Add(frame.trigger);
			return ret;
		}
		public List<UVType> GetFrameUVTypeList()
		{
			var ret = new List<UVType>();
			foreach (var frame in frames) ret.Add(frame.GetUVType());
			return ret;
		}

		public float GetAnimTimer()
		{
			return framesLength * frameRate;
		}
		public float GetFrameRateOriginal()
		{
			return frameRateOriginal;
		}
		public void SetFrameRateOriginal(float rate)
		{
			frameRateOriginal = rate;
			frameRate = rate;
		}
		public void TestFirstFrameTrigger()
		{
			if (!string.IsNullOrEmpty(frames[currentFrame].trigger))
				if (onAnimTrigger != null)
					onAnimTrigger(frames[currentFrame].trigger);
		}
		public bool IsBodyPartPointer()
		{
			return bodyPart.preset == BodyPart.Pointer_1 ||
			       bodyPart.preset == BodyPart.Pointer_2 ||
			       bodyPart.preset == BodyPart.Pointer_3;
		}
		public bool HasVariableSortingOrder()
		{
			if (defaultHasVariableSortingOrder) return true;
			var initSortingOrder = frames[0].sortingOrder;
			foreach (var frame in frames)
				if (frame.sortingOrder !=
				    initSortingOrder) // Different sorting order
					return true;
			return false;
		}
		public void Reset()
		{
			currentFrame = 0;
			currentAnimFrame = frames[currentFrame];
			looped = false;
		}
		public void SetAllFramesSortingOrder(int sortingOrder)
		{
			foreach (var frame in frames) frame.sortingOrder = sortingOrder;
		}
		public void SetUseUnscaledTime(bool useUnscaledTime)
		{
			this.useUnscaledTime = useUnscaledTime;
		}
		public void SetFrameRateMod(float frameRateMod)
		{
			frameRate /= frameRateMod;
			if (frameRateMod > 0 && (frameRate <= 0.00001f ||
			                         float.IsNaN(frameRate) ||
			                         float.IsInfinity(frameRate)))
				frameRate = 0.00001f;
		}
		public void SetFrameRateMod_DontStack(float frameRateMod)
		{
			frameRate = frameRateOriginal / frameRateMod;
		}
		public void SetCurrentFrame(int currentFrame)
		{
			this.currentFrame = currentFrame;
		}
		public V_Skeleton_Frame GetCurrentFrame()
		{
			return currentAnimFrame;
		}
		public int GetCurrentFrameNumberIndex()
		{
			return currentFrame;
		}
		public int GetCurrentFrameNumber()
		{
			return currentFrame + 1;
		}
		public int GetTotalFrameCount()
		{
			return frames.Length;
		}
		public void ModifyCurrentFrameScaleX(float scaleX)
		{
			currentAnimFrame.SetScaleX(scaleX);
		}
		public void ModifyCurrentFrameScaleY(float scaleY)
		{
			currentAnimFrame.SetScaleY(scaleY);
		}
		public void ModifyCurrentFramePos(Vector3 newPos)
		{
			currentAnimFrame.SetNewPos(newPos);
		}
		public void SetCurrentFrameSize(float newSize)
		{
			currentAnimFrame.SetNewSize(newSize);
		}
		public void SetCurrentFrameRotation(float newRot)
		{
			currentAnimFrame.SetNewRotation(newRot);
		}

		public void AddKeyframeFirstToEnd()
		{
			var newFrames = new List<V_Skeleton_Frame>(frames);

			var keyframes = GetKeyframes();
			var firstKeyframe = keyframes[0];
			var cloned = firstKeyframe.CloneNew();
			newFrames.Add(cloned);
			SetFrames(newFrames.ToArray());
			currentFrame = Array.IndexOf(frames, cloned);
			currentAnimFrame = newFrames[currentFrame];

			RemakeTween();
		}
		public void AddKeyframe()
		{
			var newFrames = new List<V_Skeleton_Frame>(frames);
			if (currentAnimFrame.frameCount != -1)
			{
				//Is keyframe, duplicate this one
				var cloned = currentAnimFrame.CloneNew();
				newFrames.Insert(newFrames.IndexOf(currentAnimFrame) + 1,
						cloned);
				SetFrames(newFrames.ToArray());
				currentFrame = Array.IndexOf(frames, cloned);
				currentAnimFrame = newFrames[currentFrame];
			}
			else
			{
				//Not keyframe, clone last keyframe
				var keyframes = GetKeyframes();
				var cloned = keyframes[keyframes.Count - 1].CloneNew();
				newFrames.Add(cloned);
				SetFrames(newFrames.ToArray());
				currentFrame = Array.IndexOf(frames, cloned);
				currentAnimFrame = newFrames[currentFrame];
			}
			RemakeTween();
		}
		public void DeleteKeyframe()
		{
			var keyframes = GetKeyframes();
			if (currentAnimFrame.frameCount != -1)
			{
				//Is keyframe, delete
				keyframes.Remove(currentAnimFrame);
				SetFrames(keyframes.ToArray());
				RemakeTween();
			}
		}
		public void ModifyFrameCount(int frameCount)
		{
			currentAnimFrame.frameCount = frameCount;
			RemakeTween();
		}
		public void SelectFrame(int frameIndex)
		{
			currentFrame = frameIndex;
			currentAnimFrame = frames[currentFrame];
		}
		public void SelectKeyframeRight()
		{
			//Look for next keyframe
			if (GetKeyframes().Count > 1)
			{
				//Theres more than one Keyframe
				var newFrameIndex = (currentFrame + 1) % frames.Length;
				while (newFrameIndex != currentFrame)
				{
					var frame = frames[newFrameIndex];
					if (frame.frameCount != -1)
					{
						//Is keyframe, select this one
						currentFrame = newFrameIndex;
						currentAnimFrame = frames[currentFrame];
						break;
					}
					newFrameIndex = (newFrameIndex + 1) % frames.Length;
				}
			}
		}
		public void SelectKeyframeLeft()
		{
			//Look for next keyframe
			if (GetKeyframes().Count > 1)
			{
				//Theres more than one Keyframe
				var newFrameIndex = currentFrame - 1;
				if (newFrameIndex < 0) newFrameIndex = frames.Length - 1;
				while (newFrameIndex != currentFrame)
				{
					var frame = frames[newFrameIndex];
					if (frame.frameCount != -1)
					{
						//Is keyframe, select this one
						currentFrame = newFrameIndex;
						currentAnimFrame = frames[currentFrame];
						break;
					}
					newFrameIndex--;
					if (newFrameIndex < 0) newFrameIndex = frames.Length - 1;
				}
			}
		}
		public V_Skeleton_Anim CloneDeep()
		{
			// Deep copy, clones every individual frame
			var framesList = new List<V_Skeleton_Frame>();
			foreach (var frame in frames)
				framesList.Add(frame.Clone());
			return new V_Skeleton_Anim(framesList.ToArray(), bodyPart,
					frameRate);
		}
		public V_Skeleton_Anim Clone()
		{
			// Shallow Copy, does not clone frames
			return new V_Skeleton_Anim(frames, bodyPart, frameRate);
		}
		public V_Skeleton_Anim CloneOnlyKeyframes()
		{
			return new V_Skeleton_Anim(CloneKeyframes().ToArray(), bodyPart,
					frameRate);
		}
		public List<V_Skeleton_Frame> CloneKeyframes()
		{
			var ret = new List<V_Skeleton_Frame>();
			var keyframes = GetKeyframes();
			foreach (var frame in keyframes) ret.Add(frame.Clone());
			return ret;
		}
		public List<V_Skeleton_Frame> GetKeyframes()
		{
			var keyframes = new List<V_Skeleton_Frame>();
			foreach (var frame in frames)
				if (frame.frameCount != -1)
					keyframes.Add(frame);
			return keyframes;
		}
		public void RemakeTween()
		{
			var keyframes = GetKeyframes();
			foreach (var keyframe in keyframes) keyframe.RefreshVertices();
			SetFrames(V_Skeleton_Frame.Smooth(keyframes.ToArray()));
			if (keyframes.IndexOf(currentAnimFrame) !=
			    -1) //Currently selected keyframe, keep selection
				currentFrame = Array.IndexOf(frames, currentAnimFrame);
			else
				currentFrame = 0;
			currentAnimFrame = frames[currentFrame];
		}
		public void MoveKeyframeLeft()
		{
			var keyframes = GetKeyframes();
			var changeIndex = keyframes.IndexOf(currentAnimFrame);
			if (changeIndex > 0)
			{
				keyframes[changeIndex] = keyframes[changeIndex - 1];
				keyframes[changeIndex - 1] = currentAnimFrame;
			}
			SetFrames(keyframes.ToArray());
			RemakeTween();
		}
		public void MoveKeyframeRight()
		{
			var keyframes = GetKeyframes();
			var changeIndex = keyframes.IndexOf(currentAnimFrame);
			if (changeIndex < keyframes.Count - 1)
			{
				keyframes[changeIndex] = keyframes[changeIndex + 1];
				keyframes[changeIndex + 1] = currentAnimFrame;
			}
			SetFrames(keyframes.ToArray());
			RemakeTween();
		}
		public void RemoveKeyframesExceptFirst()
		{
			var keyframes = GetKeyframes();
			SetFrames(new[] { keyframes[0] });
			RemakeTween();
		}







		public static string Save_Static(V_Skeleton_Anim single)
		{
			return single.Save();
		}
		public string Save()
		{
			// Returns a string to be used in savefiles
			string[] content =
			{
				"" + bodyPart.Save(),
				"",
				"" + frameRate,
				V_Animation.Save_Array(frames, V_Skeleton_Frame.Save_Static,
						"#SKELETONFRAMELIST#")
			};
			return string.Join("#SKELETONANIM#", content);
		}
		public static V_Skeleton_Anim Load(string save)
		{
			var content = V_Animation.SplitString(save, "#SKELETONANIM#");
			var bodyPart = BodyPart.Load(content[0]);
			var frameRate = float.Parse(content[2]);
			var frames = V_Animation.Load_Array(content[3],
					V_Skeleton_Frame.Load, "#SKELETONFRAMELIST#");

			return new V_Skeleton_Anim(frames, bodyPart, frameRate);
		}
	}

}
