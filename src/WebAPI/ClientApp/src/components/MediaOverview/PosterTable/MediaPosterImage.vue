<template>
	<QGlowContainer>
		<QHover
			v-if="imageUrl"
			class="media-poster">
			<template #default="{ hover }">
				<q-img
					loading="eager"
					:src="imageUrl"
					fit="fill"
					no-spinner
					class="media-poster--image"
					:alt="mediaItem.title">
					<template #default>
						<!--	Overlay	-->
						<div :class="['media-poster--overlay', hover && overlay ? 'on-hover' : '', 'white--text']">
							<MediaPosterImageContent
								:media-item="mediaItem"
								:actions="actions"
								:all-media-mode="allMediaMode"
								@action="$emit('action', $event)" />
						</div>
					</template>
					<template #error>
						<!--	Show fallback image	-->
						<MediaPosterImageContent
							fallback
							:actions="actions"
							:media-item="mediaItem"
							:all-media-mode="allMediaMode"
							@action="$emit('action', $event)" />
					</template>
				</q-img>
			</template>
		</QHover>
		<!--	Show fallback image	-->
		<MediaPosterImageContent
			v-else
			fallback
			:actions="actions"
			:media-item="mediaItem"
			:all-media-mode="allMediaMode"
			@action="$emit('action', $event)" />
	</QGlowContainer>
</template>

<script setup lang="ts">
import Log from 'consola';
import { toFullThumbUrl } from '@composables/conversion';
import type { PlexMediaSlimDTO } from '@dto';
import { useServerConnectionStore, useSettingsStore } from '#imports';

const connectionStore = useServerConnectionStore();
const settingsStore = useSettingsStore();

const props = withDefaults(defineProps<{
	mediaItem: PlexMediaSlimDTO;
	overlay?: boolean;
	actions?: boolean;
	allMediaMode?: boolean;

	thumbWidth?: number;
	thumbHeight?: number;
}>(), {
	overlay: false,
	actions: false,
	allMediaMode: false,
	thumbWidth: 200,
	thumbHeight: 300,
});

defineEmits<{
	(e: 'action', event: 'download' | 'open-media-details'): void;
}>();

const imageUrl = computed((): string => {
	if (!props.mediaItem?.hasThumb) {
		return '';
	}

	const connection = connectionStore.chooseServerConnection(props.mediaItem.plexServerId);
	if (!connection) {
		Log.error('No connection found for plexServerId in media item', props.mediaItem.plexServerId);
		return '';
	}

	const useLowQualityPoster = settingsStore.generalSettings.useLowQualityPosterImages;

	return toFullThumbUrl({
		connectionUrl: connection.url,
		mediaKey: props.mediaItem.key,
		MetaDataKey: props.mediaItem.metaDataKey,
		token: props.mediaItem.plexToken,
		width: useLowQualityPoster ? props.thumbWidth : 627,
		height: useLowQualityPoster ? props.thumbHeight : 938,
	});
});
</script>

<style lang="scss">
@import '@/assets/scss/_mixins.scss';

.q-img__content > div {
  padding: 0;
}

.media-poster {
  @extend .background-sm;

  width: 200px;

  &--image {
    height: 300px;
    padding: 0;
  }

  &--overlay {
    @extend .background-xl;
    width: 100%;
    height: 100%;
    opacity: 0;
    margin: 0;
    transition: opacity 0.2s ease-in-out;

    &.on-hover {
      opacity: 0.8;

      .q-btn {
        opacity: 1;
      }
    }
  }

}
</style>
