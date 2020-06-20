using UnityEngine;
using System;
using System.Drawing;

namespace Assets {
  public class Terrain {

    private Point _location;

    //public Dictionary<Point, Chunk> Chunks { get; } = new Dictionary<Point, Chunk>();
    private const int _ACTIVE_CHUNK_AMOUNT = 25;
    public Chunk[] Chunks = new Chunk[_ACTIVE_CHUNK_AMOUNT]; //todo: implement sometime

    public Terrain(Vector3 playerLocation) {
      var location = _GetChunkLocation(playerLocation);
      this._location = location;
      this._CreateFirstChunks();
    }


    public void Update(Vector3 playerLocation) {
      var newLocation = _GetChunkLocation(playerLocation);
      if (newLocation == this._location)
        return;

      this._location = newLocation;
      this._ManageChunks();
    }
    private void _ManageChunks() {
      var newChunks = this._CreateNewChunks();
      this._DeleteNonActiveChunks(newChunks);
      this.Chunks = newChunks;
    }

    private void _CreateFirstChunks() {
      var currentLoc = this._location;
      var chunks = this.Chunks;
      var i = 0;
      for (var y = currentLoc.Y - 2; y <= currentLoc.Y + 2; ++y)
        for (var x = currentLoc.X - 2; x <= currentLoc.X + 2; ++x) {
          var location = new Point(x * Chunk.WIDTH, y * Chunk.DEPTH);
          var chunk = new Chunk(location);
          chunk.Create();
          chunks[i++] = chunk;
        }
    }

    private Chunk[] _CreateNewChunks() {
      var newChunks = new Chunk[_ACTIVE_CHUNK_AMOUNT];
      var currentLoc = this._location;
      var i = 0;

      //checks the 9 tiles and creates non-existing chunks
      for (var y = currentLoc.Y - 2; y <= currentLoc.Y + 2; ++y)
        for (var x = currentLoc.X - 2; x <= currentLoc.X + 2; ++x) {
          var location = new Point(x * Chunk.WIDTH, y * Chunk.DEPTH);
          var chunkExists = false;

          foreach (var chunk in this.Chunks) {
            if (chunk.Location == location) {
              chunkExists = true;
              newChunks[i++] = chunk;

            }
          }

          if (!chunkExists) {
            var chunk = new Chunk(location);
            chunk.Create();
            newChunks[i++] = chunk;
          }
        }

      return newChunks;
    }

    private void _DeleteNonActiveChunks(Chunk[] newChunks) {
      foreach (var chunk in this.Chunks) {
        var delete = true;

        foreach (var newChunk in newChunks) {
          if (newChunk == chunk)
            delete = false;
        }

        if (delete) 
          chunk.Destroy();
      }
    }

    //todo: maybe round b4 of casting
    private static Point _GetChunkLocation(Vector3 location)
      => new Point(((int)Math.Round(location.x) / Chunk.WIDTH) - 1,
        ((int)Math.Round(location.z) / Chunk.DEPTH));

  }
}