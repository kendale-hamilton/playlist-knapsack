import { playlistDuration } from "@/app/helpers/time-functions";
import { FullPlaylist } from "@/types/Playlist";
import { Track } from "@/types/Track";
import {
  Button,
  Input,
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
} from "@heroui/react";
import { useState } from "react";

type PlaylistDetailSelectorProps = {
  isOpen: boolean;
  onClose: () => void;
  id: string;
  tracks: Track[];
  setPlaylist: (playlist: FullPlaylist) => void;
};

export default function PlaylistDetailSelector(
  props: PlaylistDetailSelectorProps
) {
  const { isOpen, onClose, id, tracks, setPlaylist } = props;
  const length = playlistDuration(tracks);

  const emptyPlaylist: FullPlaylist = {
    details: {
      name: "",
      seconds: length,
      images: [
        {
          url: "",
          width: 0,
          height: 0,
        },
      ],
      id: id,
      description: "",
      spotify_url: "",
    },
    tracks: tracks,
  };
  const [newPlaylist, setNewPlaylist] = useState<FullPlaylist>(emptyPlaylist);

  const setName = (name: string) => {
    setNewPlaylist({
      ...newPlaylist,
      details: {
        ...newPlaylist.details,
        name: name,
      },
    });
  };

  const setDescription = (description: string) => {
    setNewPlaylist({
      ...newPlaylist,
      details: {
        ...newPlaylist.details,
        description: description,
      },
    });
  };

  const handleSave = () => {
    setPlaylist(newPlaylist);
    setNewPlaylist(emptyPlaylist);
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} placement="center">
      <ModalContent className="mx-4">
        <ModalHeader className="flex flex-col gap-1 text-white">
          Enter Your Playlist Details
        </ModalHeader>
        <ModalBody>
          <div className="flex flex-col space-y-4 caret-white">
            <Input
              label="Name"
              placeholder="Enter your new playlist name"
              type="text"
              value={newPlaylist.details.name}
              onValueChange={(value) => setName(value)}
              isRequired
            />
            <Input
              label="Description"
              placeholder="Enter your new playlist description"
              type="text"
              value={newPlaylist.details.description}
              onValueChange={(value) => setDescription(value)}
              isClearable
            />
            {/*Figure out how to have the user upload an image */}
            {/* <Input type="file" label="Playlist Image" placeholder="Upload a custom image file" onChange={handleFileChange} /> */}
          </div>
        </ModalBody>
        <ModalFooter>
          <Button color="danger" variant="light" onPress={onClose}>
            Cancel
          </Button>
          <Button
            color="primary"
            onPress={handleSave}
            isDisabled={!newPlaylist.details.name}
          >
            Save to Spotify
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
}
