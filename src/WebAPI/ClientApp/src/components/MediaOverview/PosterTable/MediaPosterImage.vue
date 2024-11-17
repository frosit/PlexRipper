<template>
	<QRow
		justify="center"
		align="center"
		style="height: 100%"
		:class="{ 'media-poster--fallback': fallback }">
		<QCol
			v-if="fallback"
			cols="auto">
			<QMediaTypeIcon
				class="mx-3"
				:size="90"
				:media-type="mediaType" />
		</QCol>
		<QCol
			cols="12"
			text-align="center">
			<QText
				:value="mediaItem.title"
				bold="bold"
				align="center"
				size="h6"
				class="media-poster--title" />
			<QText
				v-if="allMediaMode"
				align="center"
				:value="serverStore.getServerName(mediaItem.plexServerId)" />
		</QCol>
		<!-- Poster Actions -->
		<QCol cols="auto">
			<BaseButton
				icon="mdi-download"
				size="xl"
				flat
				:outline="false"
				@click="$emit('download')" />
			<BaseButton
				v-if="mediaType === PlexMediaType.TvShow"
				icon="mdi-magnify"
				:outline="false"
				size="xl"
				flat
				@click="$emit('open-media-details', mediaItem)" />
		</QCol>
	</QRow>
</template>

<script setup lang="ts">
import { type PlexMediaSlimDTO, PlexMediaType } from '@dto';
import { useServerStore } from '@store';

const serverStore = useServerStore();

const props = withDefaults(defineProps<{
	mediaItem: PlexMediaSlimDTO;
	fallback?: boolean;
	allMediaMode?: boolean;
}>(), {
	fallback: false,
	allMediaMode: false,
});

defineEmits<{
	(e: 'download'): void;
	(e: 'open-media-details', payload: PlexMediaSlimDTO): void;
}>();

const mediaType = computed(() => props.mediaItem?.type ?? PlexMediaType.Unknown);
</script>

<style lang="scss">
@import '@/assets/scss/_mixins.scss';

.media-poster {
  &--fallback {
    background-color: transparent !important;

    & > div {
      margin: 16px 0;
    }
  }

}
</style>
